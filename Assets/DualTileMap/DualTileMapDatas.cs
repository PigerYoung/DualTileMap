using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GridDatas
{
    public class DualTileMapDatas: MonoBehaviour
    {
        [SerializeField,LabelText("数据层数据")]
        private List<Vector3> _data = new();

        public List<Vector3> data => _data;

        [SerializeField,LabelText("显示层tileMap")]
        private Tilemap _displayTilemap;

        [SerializeField,LabelText("多层瓦片配置文件")]
        private DualViewTileConfig _config;

        public void SetData(Vector3 pos)
        {
            if(!_data.Contains(pos))
                _data.Add(pos);
        }

        public void RemoveData(Vector3 pos)
        {
            _data.Remove(pos);
        }

        public void SetTileByRule(Vector3 displayPoint,List<bool> rule)
        {
            if (_displayTilemap == null)
            {
                Debug.LogError("显示层Tilemap组件为空，请配置");
                return;
            }
            if (_config == null)
            {
                Debug.LogError("瓦片配置文件为空，请配置");
                return;
            }
            var tile = _config.GetTile(rule.ToArray());
            
            var nullFlag = rule.All(e => !e);
            var v3Int = new Vector3Int((int)displayPoint.x, (int)displayPoint.y,0);
            _displayTilemap.SetTile(v3Int, nullFlag ? null : tile);
        }
    }
}