using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif
using UnityEngine;

namespace GridDatas
{
    [Serializable]
    // 规则类（只需要四个布尔值就可以确定一个瓦片）
    public class NeighborRule
    {
        public bool[] datas = {false,false,false,false};
        
    }
    // 多层瓦片配置数据类(单个数据类)
    [Serializable]
    public class DualViewTileConfigData
    {

        [LabelText("锁定"),HorizontalGroup("HGroup")]
        [LabelWidth(30)]
        public bool Lock;
        
        [HorizontalGroup("HGroup")]
        [DisableIf("Lock")]
        public GameObject gameObject;

        [HorizontalGroup("HGroup")]
        [CustomValueDrawer(nameof(DrawMathRule))]
        [DisableIf("Lock")]
        public NeighborRule mathRule = new();
        
        [HorizontalGroup("HGroup")]
        [DisableIf("Lock")]
        public Sprite tileSp;

#if UNITY_EDITOR
        private NeighborRule DrawMathRule(NeighborRule value,GUIContent label)
        {
            var cellSize = 22;
            var gridSize = cellSize * 2;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var grid = GUILayoutUtility.GetRect(gridSize, gridSize);
            GUILayout.EndHorizontal();
            
            // 背景格+icon
            var index = 0;
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    var rect= new Rect(grid.x+c*cellSize, grid.y+r*cellSize, cellSize, cellSize);
                    EditorGUI.DrawRect(rect, new Color(0.16f, 0.16f, 0.16f));
                    
                    var haveData = value.datas[index];
                    var iconRect =  new Rect(rect.x+1f, rect.y+1f, cellSize-2f, cellSize-2f);
                    var icon = haveData ? SdfIconType.Check : SdfIconType.X;
                    var color = haveData ? Color.green : Color.red;
                    SdfIcons.DrawIcon(iconRect,icon,color);
                    index++;
                }
            }
            
            // 边界线
            for (int r = 0; r < 3; r++)
            {
                var rect= new Rect(grid.x, grid.y+r*cellSize, gridSize, 1);
                EditorGUI.DrawRect(rect, Color.gray);
            }
            for (int c = 0; c < 3; c++)
            {
                var rect= new Rect(grid.x+c*cellSize, grid.y, 1, cellSize*2);
                EditorGUI.DrawRect(rect, Color.gray);
            }
            return value;
        }    
#endif
      

        public void SetRule(bool[] rule)
        {
            mathRule.datas = rule;
        }
    }
}