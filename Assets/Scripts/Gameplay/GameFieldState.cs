using System;
using Enums;
using Gameplay.Structs;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay
{
    public struct GameFieldState : INetworkSerializable, IEquatable<GameFieldState>
    {
        private GameMarkType[,] _gridValues;
        public GameMarkType[,] GridValues => _gridValues;

        public int GridSize => _gridSize;
        private int _gridSize;
        
        public GameFieldState(int gridSize)
        {
            _gridValues = new GameMarkType[gridSize, gridSize];
            _gridSize = gridSize;
        }

        public void SetValue(Coord coord, GameMarkType markType)
        {
            try
            {
                _gridValues[coord.x, coord.y] = markType;
               
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong while trying to update Grid value!\n" + e.Message);
            }
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            
            serializer.SerializeValue(ref _gridSize);

            //array
            if (serializer.IsReader)
            {
                _gridValues = new GameMarkType[_gridSize, _gridSize];
            }

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    serializer.SerializeValue(ref _gridValues[i,j]);
                }
            }
        }

        public bool Equals(GameFieldState other)
        {
            if (_gridSize != other._gridSize)
            {
                return false;
            }
            
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    if (_gridValues[i, j] != other._gridValues[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        } 
    }
}