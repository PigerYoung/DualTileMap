
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridDatas
{
    // 如果把DualViewTileConfig和DualViewTileConfigData都放入Editor文件夹中，会导致
    // DualTileMapDatas的引用问题，DualTileMapDatas又不能放在Editor文件夹中，因此才使用宏定义划分
#if UNITY_EDITOR
    
    // 多层瓦片配置
     public class DualViewTileConfig: ScriptableObject
    {
            
        [MenuItem("Tools/DualViewTile")]
        public static void Create()
        {
            var asset = CreateInstance<DualViewTileConfig>();
            
            AssetDatabase.CreateAsset(asset,"Assets/Builds/Scene/UP_SLG/DualViewTileConfig.asset");
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [LabelText("纹理")]
        public Texture2D Text;

        [LabelText("基础瓦片")]
        public Tile Tile;

        [LabelText("手动编辑"),PropertyOrder(-1)]
        public bool EditFlag = false;

        [EnableIf("EditFlag")]
        [ListDrawerSettings(ElementColor = @"GetConfigColor",OnBeginListElementGUI = @"LockConfigDrag")]
        public List<DualViewTileConfigData> configs = new();
        
        private void OnEnable()
        {
            CheckConfigsAvailable();
        }

        // 判断该配置是否有效
        private bool CheckConfigsAvailable()
        {
            if (configs.Count !=16)
            {
                Debug.LogError($"{name}配置数据不足,请检查");
                return false;
            }


            var result = new List<bool[]>();
            for (int i = 0; i < 16; i++)
            {
                var target = new bool[4];
                for (int j = 0; j < 4; j++)
                {
                    var t = (1 << (3 - j));
                    var c = i & (1 << (3 - j));
                    target[j] = (i & (1 << (3 - j))) != 0;
                }
                result.Add(target);
            }

            for (int i = 0; i < configs.Count; i++)
            {
                var rule = configs[i].mathRule.datas;
                var flag = result.Exists(x => x.SequenceEqual(rule));
                if (!flag)
                {
                    Debug.LogError($"配置数据错误不存在{rule},请检查");
                    return false;
                }
            }
            
            return true;
        }

        [Button("检查规则"),PropertyOrder(-1)]
        private void CheckRule()
        {
           var flag=CheckConfigsAvailable();
           if (flag)
           {
               Debug.Log("规则正确");
           }
        }

        [Button("重置规则"),PropertyOrder(-1)]
        private void ResetRule()
        {
            configs.Clear();
            for (int i = 0; i < 16; i++)
            {
                var target = new bool[4];
                for (int j = 0; j < 4; j++)
                {
                    target[j] = (i & (1 << (3 - j))) != 0;
                }
                var config = new DualViewTileConfigData();
                config.SetRule(target);
                configs.Add(config);
            }
            AssetDatabase.SaveAssets();
        }

        [Button("填充图片"),PropertyOrder(-1)]
        private void FillTileSp()
        {
            if (Text == null)
            {
                Debug.LogError("纹理图为空");
                return;
            }

            var path = AssetDatabase.GetAssetPath(Text);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            
            sprites = sprites.OrderByDescending(e =>e.textureRect.y).ThenBy(e=>e.textureRect.x).ToArray();
            
            if (sprites.Count() != 16)
            {
                Debug.LogError($"填充失败，请检查{Text}的精灵图数量是否正确");
                return;
            }

            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                config.tileSp = sprites[i];
            }
            
        }
        
        private Color GetConfigColor(int index,Color color)
        {
            if (index < 0 || index >= configs.Count) return color;

            var targetColor = configs[index].Lock ?  new Color(0.45f, 0.22f, 0.22f) : color;
            return targetColor;
        }

        private void LockConfigDrag(int index)
        {
            if(index>=configs.Count||!configs[index].Lock)return;

            var evt = Event.current;
            if (evt.type != EventType.MouseDown || evt.button != 0)
                return;
            var row = GUIHelper.GetCurrentLayoutRect();
            var handle = new Rect(row.x + 4f, row.y, 20f, row.height);
            if (handle.Contains(evt.mousePosition))
                evt.Use();
        }
        
        
        private Sprite GetTileSprite(bool[] rule)
        {
            if (rule.Length != 4)
            {
                Debug.LogError("规则数据错误，获取精灵图失败");
                return null;
            }

            var config = configs
                .FirstOrDefault(x => x.mathRule.datas.SequenceEqual(rule));
            if (config==null)
            {
                Debug.LogError($"没有对应规则{rule[0]}{rule[1]}{rule[2]}{rule[3]}配置，获取精灵图失败");
                return null;
            }

            if (config.tileSp == null)
            {
                Debug.LogError($"规则{rule[0]}{rule[1]}{rule[2]}{rule[3]}配置的精灵图为空，获取精灵图失败");
                return null;
            }
            return config.tileSp;
        }
        
        // ----------------------public---------------
        public Tile GetTile(bool[] rule)
        {
            if (Tile == null)
            {
                Debug.LogError("DualViewTileConfig中配置基础瓦片为空，检查");
                return null;
            }
            var sp = GetTileSprite(rule);
            var tile = Instantiate(Tile);
            tile.sprite = sp;
            return tile;
        }
    }
#endif
}