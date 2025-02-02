﻿using DataModel.Vehicles;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menu.Cards
{
	public class CardInStorage : MonoBehaviour, ICard, IPointerClickHandler
	{
		private IStorageSelection _selection;
		
		public CardInStorage Initialize(Vehicle vehicle, IStorageSelection selection)
		{
			Vehicle = vehicle;
			_selection = selection;
			return this;
		}
		
		public Vehicle Vehicle { get; private set; }
		
		public void OnPointerClick(PointerEventData eventData)
		{
			_selection.Select(this);
		}
	}
}