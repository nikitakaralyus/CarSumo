﻿using Menu.Vehicles.Cards;
using Menu.Vehicles.Storage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Vehicles.Layout
{
    public class CardVehicleLayoutView : VehicleLayoutView<VehicleCard>, IVehicleCardSelectHandler
    {
        [Header("View Components")]
        [SerializeField] private Transform _layoutRoot;
        [SerializeField] private LayoutGroup _contentLayoutGroup;
        
        [Header("Layout Card Driven Components")]
        [SerializeField] private VehicleStorageView _storage;
        [SerializeField] private LayoutVehicleCardAnimation _animation;
        [SerializeField] private Vector3 _cardSize = Vector3.one * 0.75f;

        private IEnumerable<VehicleCard> _layout;

        protected override Transform CollectionRoot => _layoutRoot;
        private Transform CardSelectedRoot => transform;

        private void OnDisable()
        {
            _contentLayoutGroup.enabled = true;
        }

        protected override void ProcessCreatedLayout(IEnumerable<VehicleCard> layout)
        {
            _layout = layout;

            foreach (VehicleCard card in layout)
            {
                card.transform.localScale = _cardSize;
                card.SetSelectHandler(this);
            }
        }

        public void OnCardSelected(VehicleCard card)
        {
            _contentLayoutGroup.enabled = false;
            card.transform.SetParent(CardSelectedRoot);
            _storage.Enable();
            _animation.ApplyIncreaseSizeAnimation(card.transform);
            
            NotifyOtherCards(card);
        }

        public void OnCardDeselected(VehicleCard card)
        {
            _storage.Disable();
            OnCardDeselectedInternal(card);
        }

        private void NotifyOtherCards(VehicleCard selectedCard)
        {
            IEnumerable<VehicleCard> otherCards = _layout.Where(card => card != selectedCard);

            foreach (VehicleCard card in otherCards)
            {
                card.NotifyBeingDeselected(true);
                OnCardDeselectedInternal(card);
            }
        }

        private void OnCardDeselectedInternal(VehicleCard card)
        {
            card.transform.SetParent(CollectionRoot);
            card.SetLatestSiblingIndex();
            _animation.ApplyDecreaseSizeAnimation(card.transform);
        }
    }
}