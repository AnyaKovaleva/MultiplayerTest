﻿using UnityEngine.UIElements;

namespace Views
{
    public abstract class View
    {
        public VisualElement Root { get; protected set; }

        public View(UIDocument document)
        {
            MapFieldsToUI(document);
            SetPlaceholders();
        }

        public abstract void MapFieldsToUI(UIDocument document);

        protected virtual void SetPlaceholders()
        {
        }
    }
}