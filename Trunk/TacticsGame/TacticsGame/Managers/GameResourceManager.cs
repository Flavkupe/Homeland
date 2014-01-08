using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Linq;
using TacticsGame.Abilities;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.Items.SpecialStats;
using TacticsGame.Crafting;
using TacticsGame.Managers;
using System;

namespace TacticsGame.Managers
{
    public class GameResourceManager
    {
        private GameResourceManager()
        {
        }

        private string[] resourceFiles = new string[]
        {
            @"Xml\Icons.xml",
            @"Xml\GameObjects.xml",
            @"Xml\Tiles.xml",            
            @"Xml\Items.xml",
            @"Xml\Misc.xml",
            @"Xml\Abilities.xml",
            @"Xml\Recipes.xml",
            @"Xml\Edicts.xml",
            @"Xml\VisualEffects.xml",
        };

        private static GameResourceManager instance = null;

        private Dictionary<string, GameResourceInfo> resources = new Dictionary<string, GameResourceInfo>();
        private Dictionary<string, IconInfo> icons = new Dictionary<string, IconInfo>();

        public static GameResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameResourceManager();
                }

                return instance;
            }
        }

        public GameResourceInfo GetResource(string resourceName)
        {
            string lowercaseResourceName = resourceName.ToLower();
            Debug.Assert(resources.ContainsKey(lowercaseResourceName), "Resource " + lowercaseResourceName + " not added!");
            return resources[lowercaseResourceName];
        }


        public GameResourceInfo GetResourceByResourceType(string objectName, ResourceType type)
        {            
            string name = type.ToString().ToLower() + "_" + objectName.ToLower();
            Debug.Assert(resources.ContainsKey(name), "Resource " + name + " not added!");
            return resources[name];
        }

        public IconInfo GetIcon(string iconName)
        {
            string key = iconName.ToLower();
            Debug.Assert(this.icons.ContainsKey(key), "Icon " + key + " not added!");
            return this.icons[key];            
        }

        private bool loaded = false;
        private bool ignoreGraphics;
        public void LoadAllResources(bool ignoreGraphics = false) 
        {
            this.ignoreGraphics = ignoreGraphics;

            Debug.Assert(!loaded, "Should only call LoadAllResources exactly once!!");
            loaded = true;

            foreach (string file in this.resourceFiles)
            {
                if (File.Exists(file))
                {
                    this.ParseFile(file);
                }
                else
                {
                    Debug.Assert(false, "couldn't find file " + file + "!");
                }
            }

            ItemManager.Instance.Initialize(this.resources.Values.ToList());
        }

        private void ParseFile(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlElement currentElement = (XmlElement)doc.DocumentElement;
            
            //TODO: make this go to the next sibling also

            if (currentElement.Name == "GameObjects")
            {
                GetResourceInfoFromFile(currentElement, "GameObjects", "GameObject");
            }
            else if (currentElement.Name == "Icons")
            {
                GetIconInfoFromFile(currentElement);
            }
            else if (currentElement.Name == "Items")
            {
                GetResourceInfoFromFile(currentElement, "Items", "Item");
            }
            else if (currentElement.Name == "MiscObjects")
            {                
                GetResourceInfoFromFile(currentElement, "MiscObjects", "MiscObject");
            }
            else if (currentElement.Name == "Tiles")
            {                
                GetResourceInfoFromFile(currentElement, "Tiles", "Tile");
            }
            else if (currentElement.Name == "VisualEffects")
            {
                GetResourceInfoFromFile(currentElement, "VisualEffects", "VisualEffect");
            }
            else if (currentElement.Name == "Abilities")
            {
                GetResourceInfoFromFile(currentElement, "Abilities", "Ability");
            }
            else if (currentElement.Name == "Recipes")
            {
                GetResourceInfoFromFile(currentElement, "Recipes", "Recipe");
            }
            else if (currentElement.Name == "Edicts")
            {
                GetResourceInfoFromFile(currentElement, "Edicts", "Edict");
            }
        }

        private GameResourceInfo GetResourceInfoObjectByElementName(string name) 
        {
            if (name == "GameObjects")
            {
                return new GameResourceInfo();                
            }
            else if (name == "Items")
            {
                return new ItemResourceInfo();                
            }
            else if (name == "MiscObjects")
            {
                return new GameResourceInfo();                
            }
            else if (name == "Tiles")
            {
                return new GameResourceInfo();                
            }
            else if (name == "VisualEffects")
            {
                return new GameResourceInfo();
            }
            else if (name == "Abilities")
            {
                return new AbilityResourceInfo();
            }
            else if (name == "Recipes")
            {
                return new RecipeResourceInfo();
            }
            else if (name == "Edicts")
            {
                return new EdictResourceInfo();
            }
            else
            {
                throw new Exception(string.Format("Unknown element: {0}", name));
            }
        }
        
        private void GetIconInfoFromFile(XmlElement currentElement)
        {
            if (currentElement != null && currentElement.Name == "Icons")
            {
                XmlNodeList elements = currentElement.GetElementsByTagName("Icon");

                if (elements != null && elements.Count > 0)
                {
                    foreach (XmlElement element in elements)
                    {
                        string id = element.GetAttribute("Id").ToLower();
                        Debug.Assert(id != null, "Icon with no Id!");
                        IconInfo iconInfo = this.GetIconInfo(element);
                        iconInfo.Id = id;
                        iconInfo.SheetImage = TextureManager.Instance.LoadTexture2D(iconInfo.SheetName);
                        this.icons[id] = iconInfo;                        
                    }
                }
            }
        }

        private void GetResourceInfoFromFile(XmlElement currentElement, string parentNodeName, string subNodeName)
        {            
            if (currentElement != null && currentElement.Name == parentNodeName)
            {               
                XmlNodeList elements = currentElement.GetElementsByTagName(subNodeName);
                if (elements != null && elements.Count > 0)
                {
                    foreach (XmlElement element in elements)
                    {
                        GameResourceInfo info = this.GetResourceInfoObjectByElementName(parentNodeName);

                        this.GetBasicResourceInfo(element, info);
                        Debug.Assert(!this.resources.ContainsKey(info.Id), "Duplicate resource!!");

                        if (subNodeName == "Ability" && info is AbilityResourceInfo)
                        {
                            this.GetSpecialAttributesForAbility(element, info as AbilityResourceInfo);
                        }                        

                        if (subNodeName == "Recipe" && info is RecipeResourceInfo)
                        {
                            this.GetSpecialAttributesForRecipe(element, info as RecipeResourceInfo);
                        }

                        if (subNodeName == "Edict" && info is EdictResourceInfo)
                        {
                            this.GetSpecialAttributesForEdict(element, info as EdictResourceInfo);
                        }

                        if (subNodeName == "Item" && info is ItemResourceInfo)
                        {
                            this.GetSpecialAttributesForItem(element, info as ItemResourceInfo);
                        }

                        this.resources[subNodeName.ToLower() + "_" + info.Id.ToLower()] = info;
                    }
                }
            }
        }

        private void GetSpecialAttributesForEdict(XmlElement currentElement, EdictResourceInfo info)
        {
            XmlNodeList description = currentElement.GetElementsByTagName("Description");
            
            if (description.Count > 0)
            {
                XmlElement element = (XmlElement)description[0];
                info.Description = element.InnerText;
            }
        }

        private void GetSpecialAttributesForRecipe(XmlElement currentElement, RecipeResourceInfo info)
        {
            info.Ingredients = new List<ItemAndCost>();
            info.Results = new List<ItemAndCost>();

            foreach (XmlElement element in currentElement.GetElementsByTagName("Ingredient"))
            {
                info.Ingredients.Add(new ItemAndCost(element.AttributeValue("Item", null), element.AttributeValueAsInt("Number", 1)));
            }

            foreach (XmlElement element in currentElement.GetElementsByTagName("Result"))
            {
                info.Results.Add(new ItemAndCost(element.AttributeValue("Item", null), element.AttributeValueAsInt("Number", 1)));
            }
        }

        private void GetSpecialAttributesForItem(XmlElement currentElement, ItemResourceInfo info)
        {
            XmlElement itemStats = (XmlElement)currentElement.GetElementsByTagName("ItemStats")[0];
            ItemType type = itemStats.AttributeValueAsEnum<ItemType>("ItemType", ItemType.Scrap);
            if (type == ItemType.Weapon)
            {
                WeaponStats weaponStats = new WeaponStats();
                XmlNodeList list = itemStats.GetElementsByTagName("WeaponStats");
                if (list.Count > 0)
                {
                    XmlElement weaponStatsElement = (XmlElement)list[0];
                    weaponStats.Attack = weaponStatsElement.AttributeValueAsInt("Attack", 1);
                    weaponStats.APCost = weaponStatsElement.AttributeValueAsInt("APCost", 1);
                    weaponStats.RangeMin = weaponStatsElement.AttributeValueAsInt("RangeMin", 1);
                    weaponStats.RangeMax = weaponStatsElement.AttributeValueAsInt("RangeMax", 1);
                    weaponStats.WeaponType = weaponStatsElement.AttributeValueAsEnum<WeaponType>("Type", WeaponType.Sword);
                    weaponStats.VisualEffects = new List<AbilityVisualEffectInfo>();
                    foreach (XmlElement visualEffectElement in weaponStatsElement.GetElementsByTagName("VisualEffect"))
                    {
                        AbilityVisualEffectInfo effectInfo = new AbilityVisualEffectInfo();
                        effectInfo.Cylces = visualEffectElement.AttributeValueAsInt("Cycles", 1);
                        effectInfo.Scale = visualEffectElement.AttributeValueAsFloat("Scale", 1.0f);
                        effectInfo.VisualEffectType = visualEffectElement.AttributeValueAsEnum<VisualEffectType>("VisualEffectType", VisualEffectType.Animation);
                        effectInfo.TargetType = visualEffectElement.AttributeValueAsEnum<AbilityTargetType>("TargetType", AbilityTargetType.Self);
                        effectInfo.EffectName = visualEffectElement.AttributeValue("EffectName", null);                        
                        weaponStats.VisualEffects.Add(effectInfo);
                    }

                }

                info.Stats = weaponStats;
            }
            else if (type == ItemType.Armor)
            {
                ArmorStats armorStats = new ArmorStats();
                XmlNodeList list = itemStats.GetElementsByTagName("ArmorStats");
                if (list.Count > 0)
                {
                    XmlElement armorStatsElement = (XmlElement)list[0];
                    armorStats.Defense = armorStatsElement.AttributeValueAsInt("Defense", 1);
                    armorStats.ArmorType = armorStatsElement.AttributeValueAsEnum<ArmorType>("Type", ArmorType.Medium);
                    armorStats.ArmorSlot = armorStatsElement.AttributeValueAsEnum<EquipmentSlot>("Slot", EquipmentSlot.Chest);
                }

                info.Stats = armorStats;
            }
            else
            {
                info.Stats = new ItemStats();
            }

            info.Stats.Type = type;
            info.Stats.Cost = itemStats.AttributeValueAsInt("Cost", 1);
            info.Stats.Rarity = itemStats.AttributeValueAsEnum<Rarity>("Rarity", Rarity.Common);
            info.Stats.Type = itemStats.AttributeValueAsEnum<ItemType>("ItemType", ItemType.Scrap);

            XmlNodeList description = itemStats.GetElementsByTagName("Description");
            if (description.Count > 0)
            {
                XmlElement element = (XmlElement)description[0];
                info.Stats.Description = element.InnerText;
            }
        }

        private void GetSpecialAttributesForAbility(XmlElement currentElement, AbilityResourceInfo info)
        {

            foreach (XmlElement element in currentElement.GetElementsByTagName("VisualEffect"))
            {
                AbilityVisualEffectInfo effectInfo = new AbilityVisualEffectInfo();
                effectInfo.Cylces = element.AttributeValueAsInt("Cycles", 1);
                effectInfo.Scale = element.AttributeValueAsFloat("Scale", 1.0f);
                effectInfo.VisualEffectType = element.AttributeValueAsEnum<VisualEffectType>("VisualEffectType", VisualEffectType.Animation);
                effectInfo.TargetType = element.AttributeValueAsEnum<AbilityTargetType>("TargetType", AbilityTargetType.Self);
                effectInfo.EffectName = element.AttributeValue("EffectName", null);
                info.VisualEffects.Add(effectInfo);
            }

            XmlElement abilityStats = (XmlElement)currentElement.GetElementsByTagName("AbilityStats")[0];
            info.Stats.APCost = abilityStats.AttributeValueAsInt("APCost", 1);
            info.Stats.MaxRange = abilityStats.AttributeValueAsInt("MaxRange", 1);
            info.Stats.MinRange = abilityStats.AttributeValueAsInt("MinRange", 1);
            info.Stats.Cooldown = abilityStats.AttributeValueAsInt("Cooldown", 0);
            info.Stats.Type = abilityStats.AttributeValueAsEnum<AbilityType>("AbilityType", AbilityType.Unknown);            

            foreach (XmlElement element in abilityStats.GetElementsByTagName("AbilityProperty"))
            {
                info.Stats.Properties |= element.AttributeValueAsEnum<AbilityProperty>("PropertyType", AbilityProperty.None);                
            }

            // Add direct effects
            foreach (XmlElement element in abilityStats.GetElementsByTagName("AbilityDirectEffect"))
            {
                AbilityEffect effect = new AbilityEffect(element.AttributeValueAsEnum<AbilityTargetType>("TargetType", AbilityTargetType.Any));
                effect.DirectEffects = element.AttributeValueAsEnum<AbilityDirectEffect>("DirectEffect", AbilityDirectEffect.None);
                effect.APModification = element.AttributeValueAsInt("APModification", 0);
                effect.HPModification = element.AttributeValueAsInt("HPModification", 0);
                effect.MoraleModification = element.AttributeValueAsInt("MoraleModification", 0);
                info.Stats.AbilityEffects.Add(effect);
            }

            // Add status effects
            foreach (XmlElement element in abilityStats.GetElementsByTagName("AbilityStatusEffect"))
            {
                AbilityEffect effect = new AbilityEffect(element.AttributeValueAsEnum<AbilityTargetType>("TargetType", AbilityTargetType.Any));
                effect.StatusEffect = new UnitStatusEffectInfo(element.AttributeValueAsEnum<UnitStatusEffect>("StatusEffect", UnitStatusEffect.ActAgain));
                effect.StatusEffect.Duration = element.AttributeValueAsInt("Duration", 1);
                effect.StatusEffect.Modifier = element.AttributeValueAsInt("Modifier", 0);                
                info.Stats.AbilityEffects.Add(effect);
            }

            // Add cooldown effects
            foreach (XmlElement element in abilityStats.GetElementsByTagName("CooldownEffect"))
            {
                AbilityEffect effect = new AbilityEffect(element.AttributeValueAsEnum<AbilityTargetType>("TargetType", AbilityTargetType.Any));
                effect.CooldownEffect = new CooldownModificationEffect();
                effect.CooldownEffect.Target = element.AttributeValueAsEnum<CooldownModificationEffect.TargetAbilityCooldown>("CooldownTarget", CooldownModificationEffect.TargetAbilityCooldown.All);
                effect.CooldownEffect.Condition = element.AttributeValueAsEnum<CooldownModificationEffect.CooldownEffectCondition>("Condition", CooldownModificationEffect.CooldownEffectCondition.None);
                effect.CooldownEffect.Modification = element.AttributeValueAsNullableInt("Modifier", null);
                effect.CooldownEffect.SpecificAbilityName = element.AttributeValue("SpecificName", null);                
                info.Stats.AbilityEffects.Add(effect);
            }

        }

        private GameResourceInfo GetBasicResourceInfo(XmlElement element, GameResourceInfo info)
        {            
            info.Id = element.GetAttribute("Id");
            string displayName = element.GetAttribute("DisplayName");
            info.DisplayName = displayName == null ? info.Id : displayName;
            info.TextureInfo = this.GetTextureInfo(element);
            return info;
        }

        private IconInfo GetIconInfo(XmlElement element)
        {
            XmlNodeList nodes = element.GetElementsByTagName("IconInfo");
            
            if (nodes == null)
            {
                return null;
            }

            XmlElement iconInfoElement = (XmlElement)nodes[0];

            string sheetName = iconInfoElement.AttributeValue("SheetName", null);
            int coordX = iconInfoElement.AttributeValueAsInt("XCoord", 0);
            int coordY = iconInfoElement.AttributeValueAsInt("YCoord", 0);
            int dimensions = iconInfoElement.AttributeValueAsInt("Dimensions", 32);
            IconInfo iconInfo = new IconInfo(sheetName, coordX, coordY, dimensions);
            return iconInfo;
        }

        /// <summary>
        /// Gets the textureinfo from the element.
        /// </summary>
        /// <param name="element">The element that contains the textureinfo element inside it, such as "GameObject"</param>
        /// <returns></returns>
        private TextureInfo GetTextureInfo(XmlElement element)
        {
            XmlNodeList nodes = element.GetElementsByTagName("TextureInfo");

            if (nodes == null)
            {
                return null;
            }

            Debug.Assert(nodes != null && nodes.Count == 1, "No texture info, or wrong node count!");
            XmlElement textureInfoElement = (XmlElement)nodes[0];            
            string textureName = textureInfoElement.AttributeValue("TextureName", null);            
            TextureInfo info = new TextureInfo(textureName ?? string.Empty);
            info.IsAnimated = textureInfoElement.AttributeValueAsBool("Animated", false);
            info.VerticalFrames = textureInfoElement.AttributeValueAsInt("HorizontalRows", 1);
            info.HorizontalFrames = textureInfoElement.AttributeValueAsInt("VerticalColumns", 1);
            info.DefaultVertical = textureInfoElement.AttributeValueAsInt("DefaultVertical", 1);
            info.StaticFrame = textureInfoElement.AttributeValueAsInt("DefaultHorizontal", 1);
            info.Width = textureInfoElement.AttributeValueAsInt("Width", 32);
            info.Height = textureInfoElement.AttributeValueAsInt("Height", 32);
            info.AnimationRate = textureInfoElement.AttributeValueAsInt("AnimationRate", 500);            
            info.Scale = textureInfoElement.AttributeValueAsFloat("Scale", 1.0f);

            info.Texture = TextureManager.Instance.LoadTexture2D(info.TextureName);

            string iconName = textureInfoElement.AttributeValue("IconName", null);

            if (iconName != null)
            {
                info.Icon = this.GetIcon(iconName);
            }
            else
            {
                // If there is no icon, just use the texture itself as the icon.
                info.Icon = new IconInfo(info.Texture, 32);
            }
                           
            return info;
        }

        /// <summary>
        /// Gets the display name if specified. Otherwise gets the id name.
        /// </summary>
        public string GetDisplayNameByResourceType(string objectName, ResourceType resourceType)
        {
            GameResourceInfo info = this.GetResourceByResourceType(objectName, resourceType);
            return string.IsNullOrWhiteSpace(info.DisplayName) ? info.Id : info.DisplayName;
        }
    }
}

