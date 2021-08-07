﻿using System.Collections.Generic;
using System.Linq;
using CarSumo.DataModel.Accounts;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Menu.Accounts
{
    public class AccountListItemDragHandler : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private IClientAccountStorageOperations _storageOperations;

        private Transform _originalParent;
        private Transform _draggingParent;

        private Account _account;
        
        [Inject]
        private void Construct(IClientAccountStorageOperations storageOperations)
        {
            _storageOperations = storageOperations;
        }

        public void Initialize(Account account, Transform originalParent, Transform draggingParent)
        {
            _account = account;
            _originalParent = originalParent;
            _draggingParent = draggingParent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(_draggingParent);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 originalPosition = transform.position;
            Vector2 dragPosition = eventData.position;
            transform.position = new Vector3(originalPosition.x, dragPosition.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IEnumerable<AccountListItemDragHandler> siblings = GetAllSiblingAccounts(_originalParent);
            AccountListItemDragHandler closest = FindClosest(siblings);

            int siblingIndex = closest is null ? 0 : closest.transform.GetSiblingIndex() + 1;
            
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(siblingIndex);

            _storageOperations.ChangeOrder(GetAllSiblingAccounts(_originalParent)
                .Select(item => item._account)
                .ToArray());
        }

        [CanBeNull]
        private AccountListItemDragHandler FindClosest(IEnumerable<AccountListItemDragHandler> items)
        {
            AccountListItemDragHandler closestItem = null;
            foreach (AccountListItemDragHandler item in items)
            {
                if (transform.position.y - item.transform.position.y < 0)
                {
                    closestItem = item;
                }
            }
            return closestItem;
        }

        private IEnumerable<AccountListItemDragHandler> GetAllSiblingAccounts(Transform layoutParent)
        {
            for (int i = 0; i < layoutParent.childCount; i++)
            {
                Transform child = layoutParent.GetChild(i);
                
                if (child.TryGetComponent<AccountListItemDragHandler>(out var sibling) == false)
                    continue;

                yield return sibling;
            }   
        }
    }
}