using System;
using System.Collections.Generic;
using Enums.UI;
using Interfaces.UI;
using UnityEngine;

using SortingLayer = Enums.UI.SortingLayer;

namespace Views
{
    public class ViewsController
    {
        private static List<IView> _views;

        private static List<SortingLayerView> _sortingLayers;

        private struct ViewState
        {
            public IView View;
            public ViewState(IView view)
            {
                View = view;
            }
        }

        private static Stack<ViewState> _history;

        private static IView _currentlyOpened;

        public static void Initialize(List<IView> views, List<SortingLayerView> sortingLayers)
        {
            DeInitialize();
            
            _views = views;
            _sortingLayers = sortingLayers;

            _history = new Stack<ViewState>();

            _currentlyOpened = null;
            
            HideAllViews();

            //HideAllLayersWithTheirContent();
        }

        public static void DeInitialize()
        {
            _views?.Clear();
            _sortingLayers?.Clear();
            _history?.Clear();
        }

        private static void HideAllViews()
        {
            foreach (var view in _views)
            {
                view.Close();
            }
        }
        
        // public static void Open(ViewType viewType)
        // {
        //     foreach (var view in _views)
        //     {
        //         if (view.Type == viewType)
        //         {
        //             _currentlyOpened?.Close();
        //
        //             view.Open();
        //
        //             AddToHistory(view);
        //
        //             if (_currentlyOpened != null)
        //             {
        //                 ManageSortingLayers(_currentlyOpened.SortingLayer, view.SortingLayer);
        //             }
        //             else
        //             {
        //                 HideSortingLayersAbove(view.SortingLayer);
        //                 ShowSortingLayer(view.SortingLayer);
        //             }
        //
        //             _currentlyOpened = view;
        //
        //             return;
        //         }
        //     }
        // }
        public static void Open(Type viewType)
        {
            foreach (var view in _views)
            {
                if (view.GetType() == viewType)
                {
                    if (view.Type != ViewType.POPUP)
                    {
                        _currentlyOpened?.Close();
                    }

                    view.Open();
                    
                    AddToHistory(view);

                    // if (_currentlyOpened != null)
                    // {
                    //     ManageSortingLayers(_currentlyOpened.SortingLayer, view.SortingLayer);
                    // }
                    // else
                    // {
                    //     HideSortingLayersAbove(view.SortingLayer);
                    //     ShowSortingLayer(view.SortingLayer);
                    // }

                    _currentlyOpened = view;

                    return;
                }
            }
        }
        
        // public static void Open(ViewType viewType, Type type)
        // {
        //     foreach (var view in _views)
        //     {
        //         if (view.Type == viewType)
        //         {
        //             if (view is IListView listView)
        //             {
        //                 if (listView.ContentType == type)
        //                 {
        //                     Debug.Log("Opening list");
        //                     _currentlyOpened?.Close();
        //                     listView.Open();
        //
        //                     AddToHistory(listView);
        //
        //                     if (_currentlyOpened != null)
        //                     {
        //                         ManageSortingLayers(_currentlyOpened.SortingLayer, listView.SortingLayer);
        //                     }
        //                     else
        //                     {
        //                         HideSortingLayersAbove(listView.SortingLayer);
        //                         ShowSortingLayer(listView.SortingLayer);
        //                     }
        //
        //                     _currentlyOpened = listView;
        //
        //                     return;
        //                 }
        //             }
        //         }
        //     }
        // }
        //
        // public static void Open(PopupType popupType)
        // {
        //     foreach (var view in _views)
        //     {
        //         if (view is IPopupView popupView)
        //         {
        //             if (popupView.PopupType == popupType)
        //             {
        //                 if (_currentlyOpened != null)
        //                 {
        //                     if (_currentlyOpened.Type == ViewType.POPUP)
        //                     {
        //                         Return();
        //                     }
        //                 }
        //
        //                 view.Open();
        //
        //                 AddToHistory(view);
        //
        //                 if (_currentlyOpened != null)
        //                 {
        //                     ManageSortingLayers(_currentlyOpened.SortingLayer, view.SortingLayer);
        //                 }
        //                 else
        //                 {
        //                     HideSortingLayersAbove(view.SortingLayer);
        //                     ShowSortingLayer(view.SortingLayer);
        //                 }
        //
        //                 _currentlyOpened = view;
        //
        //                 return;
        //             }
        //         }
        //     }
        // }
        //
        // public static void OpenMessage(string message)
        // {
        //     foreach (var view in _views)
        //     {
        //         if (view is IMessageView messageView)
        //         {
        //             if (_currentlyOpened.Type == ViewType.POPUP)
        //             {
        //                 Return();
        //             }
        //
        //             messageView.Open(message);
        //
        //             AddToHistory(view);
        //
        //             if (_currentlyOpened != null)
        //             {
        //                 ManageSortingLayers(_currentlyOpened.SortingLayer, view.SortingLayer);
        //             }
        //             else
        //             {
        //                 HideSortingLayersAbove(view.SortingLayer);
        //                 ShowSortingLayer(view.SortingLayer);
        //             }
        //
        //             _currentlyOpened = view;
        //
        //             return;
        //         }
        //     }
        // }
        //
        //

        public static void Return()
        {
            if (_history.Count == 0)
            {
                return;
            }

            _currentlyOpened?.Close();
            _history.Pop();


            if (_history.TryPeek(out var viewState))
            {
                if (_currentlyOpened is not IPopupView)
                {
                    viewState.View.Open();
                }

                // if (_currentlyOpened != null)
                // {
                //     ManageSortingLayers(_currentlyOpened.SortingLayer, viewState.View.SortingLayer);
                // }

                _currentlyOpened = viewState.View;
            }
        }

        private static void AddToHistory(IView view)
        {
            _history.Push(new ViewState(view));
        }

        // private static void ManageSortingLayers(SortingLayer current, SortingLayer newLayer)
        // {
        //     if (current == newLayer)
        //     {
        //            return;
        //     }
        //
        //     if (newLayer < current)
        //     {
        //         HideSortingLayersAbove(newLayer);
        //     }
        //
        //     ShowSortingLayer(newLayer);
        // }

        // private static void HideSortingLayersAbove(SortingLayer activeLayer)
        // {
        //     foreach (var layer in _sortingLayers)
        //     {
        //         if (layer.SortingLayer > activeLayer)
        //         {
        //             layer.Hide();
        //         }
        //     }
        // }
        //
        // public static void HideAllLayersWithTheirContent()
        // {
        //     foreach (var layer in _sortingLayers)
        //     {
        //         layer.HideEverythingInThisLayer();
        //     }
        // }
        //
        // public static void ToggleLayerVisibility(SortingLayer sortingLayer, bool isVisible)
        // {
        //     var layer = _sortingLayers.Find(layer => layer.SortingLayer == sortingLayer);
        //     layer.ToggleVisibility(isVisible);
        // }
        //
        // private static void ShowSortingLayer(SortingLayer layer)
        // {
        //     foreach (var sortingLayer in _sortingLayers)
        //     {
        //         if (sortingLayer.SortingLayer == layer)
        //         {
        //             sortingLayer.Show();
        //             return;
        //         }
        //     }
        //}

        private static void OnAndroidReturnButton()
        {
            if (_currentlyOpened is IReturnable)
            {
                Return();
            }
        }
    }
}