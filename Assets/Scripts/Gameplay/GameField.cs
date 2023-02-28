using System;
using Enums;
using Gameplay.Structs;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Gameplay
{
    public class GameField : MonoBehaviour
    {
        [SerializeField] private int _fieldSize = 3;
        [SerializeField] private RectTransform _gridContainer;
        [SerializeField] private GridEntity _gridEntityPrefab;

        private GridEntity[,] _grid;

        public static GameField Instance;


        public static int FieldSize => Instance._fieldSize;

        private void Awake()
        {
            Instance = this;
            _gridContainer.sizeDelta = new Vector2(_fieldSize * 100, _fieldSize * 100);

            _grid = new GridEntity[_fieldSize, _fieldSize];

            for (int i = 0; i < _fieldSize; i++)
            {
                for (int j = 0; j < _fieldSize; j++)
                {
                    var gridEntity = Instantiate(_gridEntityPrefab, _gridContainer.transform);
                    gridEntity.name = $"{i}{j}";
                    gridEntity.Initialize(new Coord(i, j));
                    _grid[i, j] = gridEntity;
                }
            }
        }

        public void SetGridEntityValue(Coord coord, GameMarkType mark)
        {
            Debug.Log("GRID ENTITY VALUE CHANGED");
            _grid[coord.x,coord.y].SetMark(mark);
        }

        public void DisableAllGridEntities()
        {
            for (int i = 0; i < _fieldSize; i++)
            {
                for (int j = 0; j < _fieldSize; j++)
                {
                    _grid[i,j].SetInteractable(false);
                }
            }
        }

        public void EnableEmptyGridEntities()
        {
            for (int i = 0; i < _fieldSize; i++)
            {
                for (int j = 0; j < _fieldSize; j++)
                {
                    _grid[i,j].SetInteractable(_grid[i,j].GridContent == GameMarkType.NONE);
                }
            }
        }
    }
}