using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GridDatas
{

    [CustomEditor(typeof(DualTileMapDatas))]
    public class DualTileMapDatasEditor : OdinEditor
    {
        private Vector3 _lastMousePos = Vector3.zero;
        private DualTileMapDatas _datas;
        private Grid _grid;

        private void OnEnable()
        {
            _datas = target as DualTileMapDatas;
            if (_datas == null) return;
            _grid = _datas.GetComponent<Grid>();
            if (_grid == null)
                _grid = _datas.GetComponentInParent<Grid>();
        }

        private void OnSceneGUI()
        {
            if (_datas == null || _grid == null) return;

            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            Vector3 worldPos = ray.GetPoint(10f);
            Vector3 cellPos = GridTool.WorldToGridCell(_grid, worldPos);

            if (_lastMousePos != cellPos)
                _lastMousePos = cellPos;

            Handles.color = Color.white;
            Handles.DrawPolyLine(GetDrawCorners(_lastMousePos));

            if (!e.alt && e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
            {
                if (e.shift)
                    RemoveData(_lastMousePos);
                else
                    SetGridData(_lastMousePos);

                e.Use();
            }

            if (e.type == EventType.MouseUp && e.button == 0 && !e.alt)
                e.Use();

            DrawDatas();

            SceneView.RepaintAll();
        }

        private void SetGridData(Vector3 pos)
        {
            if (_datas == null || _datas.data.Contains(pos)) return;

            _datas.SetData(pos);

            DrawDisplayLayer(pos);
        }

        private void RemoveData(Vector3 pos)
        {
            if (_datas == null || !_datas.data.Contains(pos)) return;
            _datas.RemoveData(pos);
            
            DrawDisplayLayer(pos);
        }

        private Vector3[] GetDrawCorners(Vector3 pos)
        {
            var cell = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            var bottomLeft = GridTool.GridCellToWorld(_grid, cell);
            var bottomRight = GridTool.GridCellToWorld(_grid, cell + new Vector3Int(1, 0, 0));
            var topRight = GridTool.GridCellToWorld(_grid, cell + new Vector3Int(1, 1, 0));
            var topLeft = GridTool.GridCellToWorld(_grid, cell + new Vector3Int(0, 1, 0));
            return new[] { bottomLeft, bottomRight, topRight, topLeft, bottomLeft };
        }

        // 等级四角（demo中没使用）
        private static Vector3[] GetIsometricCorner(UnityEngine.Grid grid, Vector3 pos)
        {
            var tempPosInt = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            var worldPos = GridTool.GridCellToWorld(grid, tempPosInt);
            var corners = new Vector3[5];
            var top = new Vector3(worldPos.x, worldPos.y + 0.5f, worldPos.z);
            var down = worldPos;
            var left = new Vector3(worldPos.x - 0.5f, worldPos.y + 0.25f, worldPos.z);
            var right = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.25f, worldPos.z);
            corners[0] = down;
            corners[1] = left;
            corners[2] = top;
            corners[3] = right;
            corners[4] = down;
            return corners;
        }

        private void DrawDatas()
        {
            Handles.color = new Color(0, 1, 0, 0.5f);
            foreach (var pos in _datas.data)
            {
                var corners = GetDrawCorners(pos);
                Handles.DrawAAConvexPolygon(corners);
            }
        }

        // 获取数据点的四角
        private Vector3Int[] GetSquadCorners(Vector3 dataPos)
        {
            var cell = new Vector3Int((int)dataPos.x, (int)dataPos.y, (int)dataPos.z);
            var bottomLeft = cell;
            var bottomRight = cell + new Vector3Int(1, 0, 0);
            var topRight = cell + new Vector3Int(1, 1, 0);
            var topLeft = cell + new Vector3Int(0, 1, 0);
            return new[] { topLeft, topRight,bottomLeft,bottomRight };
        }

        // 获取显示点的四角
        private Vector3[] GetDisplayCorners(Vector3 displayPos)
        {
            var bottomLeft = new Vector3(displayPos.x - 0.5f, displayPos.y - 0.5f, 0);
            var bottomRight = new Vector3(displayPos.x + 0.5f, displayPos.y - 0.5f, 0);
            var topRight =  new Vector3(displayPos.x + 0.5f, displayPos.y + 0.5f, 0);
            var topLeft =  new Vector3(displayPos.x - 0.5f, displayPos.y + 0.5f, 0);
            return new[] { topLeft, topRight,bottomLeft,bottomRight };
        }

        private void DrawDisplayLayer(Vector3 dataPos)
        {
            var displayPoints = GetSquadCorners(dataPos);
            foreach (var point in displayPoints)
            {
                var rule = GetDisplayRule(point);
                _datas.SetTileByRule(point,rule);
            }
        }
        
        // 获取显示层点对应的规则
        private List<bool> GetDisplayRule(Vector3 displayPos)
        {
            // 显示点的四个角
            var displayCorners = GetDisplayCorners(displayPos);
            var rule = new List<bool>();
            foreach (var corner in displayCorners)
            {
                var result = CheckDisplayCornerPosInData(corner);
                rule.Add(result);
            }
            return rule;
        }

        // 判断单个显示点corner的位置是否在数据层中
        private bool CheckDisplayCornerPosInData(Vector3 displayCornerPos)
        {
            // 因为显示层和数据层存在0.5的单位偏差因此这样处理
            var dataPos = new Vector3(displayCornerPos.x - 0.5f, displayCornerPos.y - 0.5f, 0);
            var flag = _datas.data.Exists(e=>e==dataPos);
            return flag;
        }

    }
}