﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Origins.Dev {
	public class WikiPageExporter : ILoadable {
		public delegate string WikiLinkFormatter(string name);
		public static DictionaryWithNull<Mod, WikiLinkFormatter> LinkFormatters { get; private set; } = new();
		static List<(Type, WikiProvider)> typedDataProviders;
		public static List<(Type type, WikiProvider provider)> TypedDataProviders => typedDataProviders ??= new();
		public delegate bool ConditionalWikiProvider(object obj, out WikiProvider provider, ref bool replaceGenericClassProvider);
		static List<ConditionalWikiProvider> conditionalDataProviders;
		public static List<ConditionalWikiProvider> ConditionalDataProviders => conditionalDataProviders ??= new();
		static HashSet<Type> interfaceReplacesGenericClassProvider;
		public static HashSet<Type> InterfaceReplacesGenericClassProvider => interfaceReplacesGenericClassProvider ??= new();
		public void Load(Mod mod) {
			LinkFormatters[Origins.instance] = (t) => {
				string formattedName = t.Replace(" ", "_");
				return $"{t} | Images/{formattedName}.png | {formattedName}";
			};
			LinkFormatters[null] = (t) => {
				string formattedName = t.Replace(" ", "_");
				return $"{t} | $default | https://terraria.wiki.gg/wiki/{formattedName}";
			};
		}
		static PageTemplate wikiTemplate;
		static DateTime wikiTemplateWriteTime;
		public static PageTemplate WikiTemplate {
			get {
				if (wikiTemplate is null || File.GetLastWriteTime(DebugConfig.Instance.WikiTemplatePath) > wikiTemplateWriteTime) {
					wikiTemplate = new(File.ReadAllText(DebugConfig.Instance.WikiTemplatePath));
					wikiTemplateWriteTime = File.GetLastWriteTime(DebugConfig.Instance.WikiTemplatePath);
				}
				return wikiTemplate;
			}
		}
		public static void ExportItemPage(Item item) {
			foreach (WikiProvider provider in GetWikiProviders(item.ModItem)) {
				if (provider.GetPage(item.ModItem) is string page) File.WriteAllText(GetWikiPagePath(provider.PageName(item.ModItem)), page);
			}
		}
		public static void ExportItemStats(Item item) {
			foreach (WikiProvider provider in GetWikiProviders(item.ModItem)) {
				foreach ((string name, JObject stats) stats in provider.GetStats(item.ModItem)) {
					File.WriteAllText(
						GetWikiStatPath(stats.name),
						JsonConvert.SerializeObject(stats.stats, Formatting.Indented)
					);
				}
			}
		}
		public static void ExportItemSprites(Item item) {
			int screenWidth = Main.screenWidth;
			int screenHeight = Main.screenHeight;
			foreach (WikiProvider provider in GetWikiProviders(item.ModItem)) {
				foreach ((string name, Texture2D texture) sprite in provider.GetSprites(item.ModItem) ?? Array.Empty<(string, Texture2D)>()) {
					string filePath = Path.Combine(DebugConfig.Instance.WikiSpritesPath, sprite.name) + ".png";
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));
					Stream stream = File.Exists(filePath) ? File.OpenWrite(filePath) : File.Create(filePath);
					sprite.texture.SaveAsPng(stream, sprite.texture.Width, sprite.texture.Height);
					sprite.texture.Dispose();
					stream.Close();
				}
			}
			Main.screenWidth = screenWidth;
			Main.screenHeight = screenHeight;
		}
		public static string GetWikiName(ModItem modItem) => modItem.Name.Replace("_Item", "");
		public static string GetWikiPagePath(string name) => Path.Combine(DebugConfig.Instance.WikiPagePath, name + ".html");
		public static string GetWikiStatPath(string name) => Path.Combine(DebugConfig.Instance.StatJSONPath, name + ".json");
		public static string GetWikiItemPath(ModItem modItem) => modItem.Texture.Replace(modItem.Mod.Name, "§ModImage§");
		public static string GetWikiItemRarity(Item item) => (RarityLoader.GetRarity(item.rare)?.Name ?? ItemRarityID.Search.GetName(item.rare)).Replace("Rarity", "");
		public void Unload() {
			wikiTemplate = null;
			LinkFormatters = null;
			typedDataProviders = null;
			conditionalDataProviders = null;
			interfaceReplacesGenericClassProvider = null;
		}
		public static LocalizedText GetDefaultMainPageText(ILocalizedModType modType) {
			string name = modType.Name;
			if (modType is ModItem modItem) name = GetWikiName(modItem);
			return Language.GetOrRegister($"WikiGenerator.{modType.Mod.Name}.{modType.LocalizationCategory}.{name}.MainText");
		}
		public static IEnumerable<(string name, LocalizedText text)> GetDefaultPageTexts(ILocalizedModType modType) {
			string baseKey = $"WikiGenerator.{modType?.Mod?.Name}.{modType?.LocalizationCategory}.{modType?.Name}.";
			return GetDefaultPageTexts(baseKey);
		}
		public static IEnumerable<(string name, LocalizedText text)> GetDefaultPageTexts(string baseKey) {
			LocalizedText text;
			bool TryGetText(string suffix, out LocalizedText text) {
				if (Language.Exists(baseKey + suffix)) {
					text = Language.GetText(baseKey + suffix);
					return true;
				}
				text = null;
				return false;
			}
			if (TryGetText("Behavior", out text)) yield return ("Behavior", text);
			if (TryGetText("DifficultyChanges", out text)) yield return ("DifficultyChanges", text);
			if (TryGetText("Tips", out text)) yield return ("Tips", text);
			if (TryGetText("Trivia", out text)) yield return ("Trivia", text);
			if (TryGetText("Notes", out text)) yield return ("Notes", text);
			if (TryGetText("Lore", out text)) yield return ("Lore", text);
			if (TryGetText("Changelog", out text)) yield return ("Changelog", text);
		}
		public static IEnumerable<WikiProvider> GetDefaultProviders(object obj) {
			Type type = obj.GetType();
			(Type type, WikiProvider provider) bestMatch = default;
			bool noClassProvider = false;
			foreach ((Type type, WikiProvider provider) item in TypedDataProviders) {
				if (item.type.IsAssignableFrom(type)) {
					if (item.type.IsInterface) {
						if (InterfaceReplacesGenericClassProvider.Contains(item.type)) noClassProvider = true;
						yield return item.provider;
					} else if (bestMatch.type is null || item.type.IsAssignableTo(bestMatch.type)) {
						bestMatch = item;
					}
				}
			}
			foreach (ConditionalWikiProvider item in ConditionalDataProviders) {
				if (item(obj, out WikiProvider provider, ref noClassProvider)) {
					yield return provider;
				}
			}
			if (noClassProvider && bestMatch.type != type) yield break;
			yield return bestMatch.provider;
		}
		public static IEnumerable<WikiProvider> GetWikiProviders(object obj) {
			if (obj is ICustomWikiStat stat) {
				return stat.GetWikiProviders();
			}
			return GetDefaultProviders(obj);
		}
		public static void AddTorchLightStats(JObject data, Vector3 light) {
			float intensity = MathF.Max(MathF.Max(light.X, light.Y), light.Z);
			if (intensity > 1) light /= intensity;
			data.Add("LightIntensity", $"{intensity * 100:0.#}%");
			data.Add("LightColor", new JArray() { light.X, light.Y, light.Z });
		}
	}
	public class PageTemplate {
		ITemplateSnippet[] snippets;
		public PageTemplate(string text) {
			snippets = Parse(text);
		}
		public string Resolve(Dictionary<string, object> context) => CombineSnippets(snippets, context);
		public static ITemplateSnippet[] Parse(string text) {
			StringBuilder currentText = new();
			int blockDepth = 0;
			List<ITemplateSnippet> snippets = new();
			void FlushText() {
				if (currentText.Length > 0) {
					snippets.Add(new PlainTextSnippit(currentText.ToString()));
					currentText.Clear();
				}
			}
			for (int i = 0; i < text.Length; i++) {
				bool append = true;
				switch (text[i]) {
					case '§':
					append = false;
					FlushText();
					i++;
					while (text[i] != '§') currentText.Append(text[i++]);
					snippets.Add(new VariableSnippit(currentText.ToString()));
					currentText.Clear();
					break;

					case '#': {
						append = false;
						FlushText();
						i++;
						bool isFor = false;
						while (!char.IsWhiteSpace(text[i])) currentText.Append(text[i++]);
						switch (currentText.ToString()) {
							case "if":
							blockDepth += 1;
							break;
							case "for":
							blockDepth += 1;
							isFor = true;
							break;
							case "endif":
							throw new ArgumentException($"invalid format, endif with no if at character {i}: {text}");
							case "endfor":
							throw new ArgumentException($"invalid format, endfor with no for at character {i}: {text}");
						}
						currentText.Clear();

						i++;
						while (text[i] != '\n') currentText.Append(text[i++]);
						string condition = currentText.ToString();
						currentText.Clear();

						while (blockDepth > 0) {
							switch (text[i]) {
								case '#': {
									int parsePos = i + 1;
									StringBuilder directiveParser = new();
									while (!char.IsWhiteSpace(text[parsePos])) directiveParser.Append(text[parsePos++]);
									switch (directiveParser.ToString()) {
										case "if":
										blockDepth += 1;
										break;
										case "for":
										blockDepth += 1;
										break;
										case "endif":
										blockDepth -= 1;
										break;
										case "endfor":
										blockDepth -= 1;
										break;
									}
									if (blockDepth != 0) goto default;
									i += (isFor ? "endfor" : "endif").Length;
									break;
								}

								default:
								currentText.Append(text[i++]);
								break;
							}
						}
						snippets.Add(
							isFor ?
							new RepeatingSnippet(condition, Parse(currentText.ToString())) :
							new ConditionalSnippit(condition, Parse(currentText.ToString()))
						);
						currentText.Clear();
					}
					break;
				}
				if (append) {
					currentText.Append(text[i]);
				}
			}
			FlushText();
			return snippets.ToArray();
		}
		static string CombineSnippets(ITemplateSnippet[] snippets, Dictionary<string, object> context) => string.Join(null, snippets.Select(s => s.Resolve(context)));
		public interface ITemplateSnippet {
			string Resolve(Dictionary<string, object> context);
		}
		public class PlainTextSnippit : ITemplateSnippet {
			readonly string text;
			public PlainTextSnippit(string text) => this.text = text;
			public string Resolve(Dictionary<string, object> context) => text;
		}
		public class ConditionalSnippit : ITemplateSnippet {
			readonly string condition;
			readonly ITemplateSnippet[] snippets;
			public ConditionalSnippit(string condition, ITemplateSnippet[] snippets) {
				this.condition = condition;
				this.snippets = snippets;
			}
			public string Resolve(Dictionary<string, object> context) {
				if (context.ContainsKey(condition)) return CombineSnippets(snippets, context);
				return null;
			}
		}
		public class RepeatingSnippet : ITemplateSnippet {
			readonly string name;
			readonly ITemplateSnippet[] snippets;
			public RepeatingSnippet(string name, ITemplateSnippet[] snippets) {
				this.name = name;
				this.snippets = snippets;
			}
			public string Resolve(Dictionary<string, object> context) {
				StringBuilder builder = new();
				if (context.TryGetValue(name, out object value) && value is IEnumerable enumerable) {
					foreach (var item in enumerable) {
						context["iteratorValue"] = item;
						builder.Append(CombineSnippets(snippets, context));
					}
				}
				return builder.ToString();
			}
		}
		public class VariableSnippit : ITemplateSnippet {
			readonly string name;
			public VariableSnippit(string name) {
				this.name = name;
			}
			public string Resolve(Dictionary<string, object> context) {
				object value = context[name];
				if (value is List<Recipe> recipes) {
					string GetItemText(Item item) {
						string text = $"[link {item.Name}]";
						if (item.ModItem?.Mod is Origins) {
							text = $"[link {item.Name} | $fromStats]";
						} else if (WikiPageExporter.LinkFormatters.TryGetValue(item.ModItem?.Mod, out var formatter)) {
							text = $"[link {formatter(item.Name)}]";
						}
						if (item.stack != 1) text += $"({item.stack})";
						return text;
					}
					StringBuilder builder = new();
					builder.AppendLine("{recipes");
					foreach (var group in recipes.GroupBy((r) => new RecipeRequirements(r))) {
						builder.AppendLine("{");
						if (group.Key.requirements.Length > 0) {
							builder.AppendLine("stations:[");
							foreach (var requirement in group.Key.requirements) {
								if (WikiPageExporter.LinkFormatters.TryGetValue(requirement.mod, out var formatter)) {
									builder.AppendLine($"[link {formatter(requirement.name)}]");
								} else {
									Origins.instance.Logger.Warn($"No wiki link formatter for mod {requirement.mod}, skipping requirement for {requirement.name}");
								}
							}
							builder.AppendLine("]");
						}
						builder.AppendLine("items:[");
						foreach (Recipe recipe in group) {
							builder.AppendLine("{");
							builder.AppendLine("result:" + GetItemText(recipe.createItem) + ",");
							builder.AppendLine("ingredients:[");
							for (int i = 0; i < recipe.requiredItem.Count; i++) {
								if (i > 0) builder.Append(",");
								builder.Append(GetItemText(recipe.requiredItem[i]));
							}
							builder.AppendLine();
							builder.AppendLine("]");
							builder.AppendLine("}");
						}
						builder.AppendLine("]");
						builder.AppendLine("}");
					}
					builder.Append("recipes}");
					return builder.ToString();
				}
				return CombineSnippets(Parse(value.ToString()), context);
			}
		}
	}
	public class RecipeRequirements {
		public readonly (Mod mod, string name)[] requirements;
		public RecipeRequirements(Recipe recipe) {
			requirements =
				recipe.requiredTile.Select(t => (TileLoader.GetTile(t)?.Mod, Lang.GetMapObjectName(MapHelper.TileToLookup(t, 0))))
				/*.Concat(
					recipe.Conditions.Select(c => c.Description.Value)
				)*/.ToArray();
		}
		public override bool Equals(object other) {
			return other is RecipeRequirements req && Equals(req);
		}
		public bool Equals(RecipeRequirements other) {
			if (other.requirements.Length != requirements.Length) return false;
			for (int i = 0; i < requirements.Length; i++) {
				if (!other.requirements.Contains(requirements[i])) return false;
			}
			return true;
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 0;
				for (int i = 0; i < requirements.Length; i++) {
					hashCode = (hashCode * 397) ^ requirements[i].GetHashCode();
				}
				return hashCode;
			}
		}
	}
	public class DictionaryWithNull<TKey, TValue> : Dictionary<TKey, TValue> {
		bool hasNullValue;
		TValue nullValue;
		public new TValue this[TKey key] {
			get {
				if (key is null) return nullValue;
				return base[key];
			}
			set {
				if (key is null) {
					nullValue = value;
					hasNullValue = true;
				} else {
					base[key] = value;
				}
			}
		}
		public new bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
			if (key is null) {
				value = nullValue;
				return hasNullValue;
			}
			return base.TryGetValue(key, out value);
		}
	}
	public interface ICustomWikiStat {
		bool Buyable => false;
		void ModifyWikiStats(JObject data) { }
		string[] Categories => Array.Empty<string>();
		bool? Hardmode => null;
		bool FullyGeneratable => true;
		bool ShouldHavePage => true;
		bool NeedsCustomSprite => false;
		LocalizedText PageTextMain => (this is ILocalizedModType modType) ? WikiPageExporter.GetDefaultMainPageText(modType) : null;
		IEnumerable<(string name, LocalizedText text)> PageTexts => (this is ILocalizedModType modType) ? WikiPageExporter.GetDefaultPageTexts(modType) : null;
		IEnumerable<WikiProvider> GetWikiProviders() => WikiPageExporter.GetDefaultProviders(this);
	}
	public class ItemWikiProvider : WikiProvider<ModItem> {
		public override string PageName(ModItem modItem) => WikiPageExporter.GetWikiName(modItem);
		public override string GetPage(ModItem modItem) {
			Item item = modItem.Item;
			Dictionary<string, object> context = new() {
				["Name"] = WikiPageExporter.GetWikiName(modItem),
				["DisplayName"] = Lang.GetItemNameValue(item.type)
			};
			(List<Recipe> recipes, List<Recipe> usedIn) = WikiExtensions.GetRecipes(item);
			if (recipes.Count > 0 || usedIn.Count > 0) {
				context["Crafting"] = true;
				if (recipes.Count > 0) {
					context["Recipes"] = recipes;
				}
				if (usedIn.Count > 0) {
					context["UsedIn"] = usedIn;
				}
			}

			IEnumerable<(string name, LocalizedText text)> pageTexts;
			if (modItem is ICustomWikiStat wikiStats) {
				context["PageTextMain"] = wikiStats.PageTextMain.Value;
				pageTexts = wikiStats.PageTexts;
			} else {
				string key = $"WikiGenerator.{modItem.Mod?.Name}.{modItem.LocalizationCategory}.{context["Name"]}.MainText";
				context["PageTextMain"] = Language.GetOrRegister(key).Value;
				pageTexts = WikiPageExporter.GetDefaultPageTexts(modItem);
			}
			foreach (var text in pageTexts) {
				context[text.name] = text.text;
			}
			return WikiPageExporter.WikiTemplate.Resolve(context);
		}
		public override IEnumerable<(string, JObject)> GetStats(ModItem modItem) {
			Item item = modItem.Item;
			JObject data = new();
			ICustomWikiStat customStat = item.ModItem as ICustomWikiStat;
			data["Image"] = WikiPageExporter.GetWikiItemPath(modItem);
			JArray types = new("Item");
			if (item.accessory) types.Add("Accessory");
			if (item.damage > 0 && item.useStyle != ItemUseStyleID.None) {
				types.Add("Weapon");
				if (!item.noMelee && item.useStyle == ItemUseStyleID.Swing) {
					types.Add("Sword");
				}
				if (item.shoot > ProjectileID.None) {
					switch (ContentSamples.ProjectilesByType[item.shoot].aiStyle) {
						case ProjAIStyleID.Boomerang:
						types.Add("Boomerang");
						break;

						case ProjAIStyleID.Spear:
						types.Add("Spear");
						break;
					}
				}
				switch (item.useAmmo) {
					case ItemID.WoodenArrow:
					types.Add("Bow");
					break;
					case ItemID.MusketBall:
					types.Add("Gun");
					break;
				}
				switch (item.ammo) {
					case ItemID.WoodenArrow:
					types.Add("Arrow");
					break;
					case ItemID.MusketBall:
					types.Add("Bullet");
					break;
				}
			}
			if (customStat?.Hardmode ?? (!item.material && !item.consumable && item.rare > ItemRarityID.Orange)) types.Add("Hardmode");

			if (item.ammo != 0) types.Add("Ammo");
			if (item.pick != 0 || item.axe != 0 || item.hammer != 0 || item.fishingPole != 0 || item.bait != 0) types.Add("Tool");
			if (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1) types.Add("Armor");
			if (item.createTile != -1) {
				types.Add("Tile");
				if (TileID.Sets.Torch[item.createTile]) {
					types.Add("Torch");
				}
			}
			if (customStat is not null) foreach (string cat in customStat.Categories) types.Add(cat);
			data.Add("Types", types);

			data.AppendStat("PickPower", item.pick, 0);
			data.AppendStat("AxePower", item.axe, 0);
			data.AppendStat("HammerPower", item.hammer, 0);
			data.AppendStat("FishPower", item.fishingPole, 0);
			data.AppendStat("BaitPower", item.bait, 0);

			if (item.createTile != -1) {
				ModTile tile = TileLoader.GetTile(item.createTile);
				if (tile is not null) {
					data.AppendStat(Main.tileHammer[item.createTile] ? "HammerReq" : "PickReq", tile.MinPick, 0);
				}
				int width = 1, height = 1;
				if (TileObjectData.GetTileData(item.createTile, item.placeStyle) is TileObjectData tileData) {
					width = tileData.Width;
					height = tileData.Height;
				}
				data.Add("PlacementSize", new JArray(width, height));
			}
			data.AppendStat("Defense", item.defense, 0);
			if (item.headSlot != -1) {
				data.Add("ArmorSlot", "Helmet");
			} else if (item.bodySlot != -1) {
				data.Add("ArmorSlot", "Shirt");
			} else if (item.legSlot != -1) {
				data.Add("ArmorSlot", "Pants");
			}
			data.AppendStat("ManaCost", item.mana, 0);
			data.AppendStat("HealLife", item.healLife, 0);
			data.AppendStat("HealMana", item.healMana, 0);
			data.AppendStat("Damage", item.damage, -1);
			if (item.damage > 0) {
				string damageClass = item.DamageType.DisplayName.Value;
				damageClass = damageClass.Replace(" damage", "");
				damageClass = Regex.Replace(damageClass, "( |^)(\\w)", (match) => match.Groups[2].Value.ToUpper());
				data.Add("DamageClass", damageClass);
			}
			data.AppendStat("Knockback", item.knockBack, 0);
			data.AppendStat("Crit", item.crit + 4, 4);
			data.AppendStat("UseTime", item.useTime, 100);
			data.AppendStat("Velocity", item.shootSpeed, 0);
			string itemTooltip = "";
			for (int i = 0; i < item.ToolTip.Lines; i++) {
				if (i > 0) itemTooltip += "\n";
				itemTooltip += item.ToolTip.GetLine(i);
			}
			data.AppendStat("Tooltip", itemTooltip, "");
			data.AppendStat("Rarity", WikiPageExporter.GetWikiItemRarity(item), "");
			if (customStat?.Buyable ?? false) data.AppendStat("Buy", item.value, 0);
			data.AppendStat("Sell", item.value / 5, 0);

			if (customStat is not null) customStat.ModifyWikiStats(data);
			data.AppendStat("SpriteWidth", item.ModItem is null ? item.width : ModContent.Request<Texture2D>(item.ModItem.Texture).Width(), 0);
			data.AppendStat("InternalName", item.ModItem?.Name, null);
			yield return (PageName(modItem), data);
		}
	}
	public interface INoSeperateWikiPage { }
	public class StatOnlyItemWikiProvider : ItemWikiProvider {
		protected override void Register() {
			base.Register();
			WikiPageExporter.TypedDataProviders.Add((typeof(INoSeperateWikiPage), this));
			WikiPageExporter.InterfaceReplacesGenericClassProvider.Add(typeof(INoSeperateWikiPage));
		}
		public override string GetPage(ModItem modItem) => null;
	}
	public abstract class WikiProvider : ModType {
		protected override void Register() {
			ModTypeLookup<WikiProvider>.Register(this);
		}
		public abstract string PageName(object value);
		public virtual string GetPage(object value) => default;
		public virtual IEnumerable<(string name, JObject stats)> GetStats(object value) => default;
		public virtual IEnumerable<(string name, Texture2D texture)> GetSprites(object value) => default;
	}
	public abstract class WikiProvider<T> : WikiProvider {
		protected override void Register() {
			base.Register();
			Type ownType = this.GetType();
			if (ownType.BaseType.IsGenericType) {
				Type baseWithGenerics = typeof(WikiProvider<>).MakeGenericType(ownType.BaseType.GenericTypeArguments);
				if (ownType.BaseType == baseWithGenerics) {
					WikiPageExporter.TypedDataProviders.Add((typeof(T), this));
				}
			}
		}
		public sealed override string PageName(object value) => value is T v ? PageName(v) : null;
		public sealed override string GetPage(object value) => value is T v ? GetPage(v) : default;
		public sealed override IEnumerable<(string, JObject)> GetStats(object value) => value is T v ? GetStats(v) : null;
		public sealed override IEnumerable<(string, Texture2D)> GetSprites(object value) => value is T v ? GetSprites(v) : null;
		public abstract string PageName(T value);
		public abstract string GetPage(T value);
		public abstract IEnumerable<(string, JObject)> GetStats(T value);
		public virtual IEnumerable<(string, Texture2D)> GetSprites(T value) => default;
	}
	public static class WikiExtensions {
		public static void AppendStat<T>(this JObject data, string name, T value, T defaultValue) {
			if (!value.Equals(defaultValue)) {
				data.Add(name, JToken.FromObject(value));
			}
		}
		public static (List<Recipe> recipes, List<Recipe> usedIn) GetRecipes(Item item) {
			List<Recipe> recipes = new();
			List<Recipe> usedIn = new();
			for (int i = 0; i < Main.recipe.Length; i++) {
				Recipe recipe = Main.recipe[i];
				if (recipe.HasResult(item.type)) {
					recipes.Add(recipe);
				}
				if (recipe.HasIngredient(item.type)) {
					usedIn.Add(recipe);
				} else {
					foreach (int num in recipe.acceptedGroups) {
						if (RecipeGroup.recipeGroups[num].ContainsItem(item.type)) {
							usedIn.Add(recipe);
						}
					}
				}
			}
			return (recipes, usedIn);
		}
	}
}