using System.Collections.Generic;
using UnityEngine;

namespace GridDatas
{
    public static class GridTool
    {
        // 世界坐标 → 网格坐标（Vector3Int）
        public static Vector3Int WorldToGridCell(UnityEngine.Grid grid,Vector3 worldPosition)
        {
            if(grid==null) return Vector3Int.zero;
            var temp= grid.WorldToCell(worldPosition);
            return new Vector3Int(temp.x, temp.y, 0);
        }
        // 网格坐标 → 世界坐标（Vector3，单元格中心）
        public static Vector3 GridCellToWorld(UnityEngine.Grid grid,Vector3Int cellPosition)
        {
            if(grid==null) return Vector3.zero;
            return grid.CellToWorld(cellPosition);
        }
        public static Vector3 GridCellToWorld(UnityEngine.Grid grid,Vector3 cellPosition)
        {
           var temp= new Vector3Int((int)cellPosition.x, (int)cellPosition.y, 0);
            if(grid==null) return Vector3.zero;
            return grid.CellToWorld(temp);
        }
        
        public static List<Vector3> GetAroundPos(Vector3 pos)
        {
            var directions = new Vector3[]
            {
                new Vector3(0, 1, 0), // 上
                new Vector3(0, -1, 0), // 下
                new Vector3(-1, 0, 0), // 左
                new Vector3(1, 0, 0), // 右
                new Vector3(1, 1, 0), // 右上
                new Vector3(1, -1, 0), // 右下
                new Vector3(-1, 1, 0), // 左上
                new Vector3(-1, -1, 0), // 左下
                new Vector3(-2, 2, 0),
                new Vector3(-1, 2, 0),
                new Vector3(0, 2, 0),
                new Vector3(1, 2, 0),
                new Vector3(2, 2, 0),
                new Vector3(-2, 1, 0),
                new Vector3(2, 1, 0),
                new Vector3(-2, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(-2, -1, 0),
                new Vector3(2, -1, 0),
                new Vector3(-2, -2, 0),
                new Vector3(-1, -2, 0),
                new Vector3(0, -2, 0),
                new Vector3(1, -2, 0),
                new Vector3(2, -2, 0)

            };
            var result = new List<Vector3>();
            foreach (var VARIABLE in directions)
            {
                var temp= pos + VARIABLE;
                result.Add(temp);
            }
            return result;
        }
        
    }
}