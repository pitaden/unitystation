﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConstructionHandler 
{
	/// <summary>
	/// Interactions the update.
	/// </summary>
	/// <returns><c>true</c>, Continue construction handler to handle the input, <c>false</c> Prevent any further Construction logic.</returns>
	/// <param name="interaction">Interaction.</param>
	/// <param name="Slot">Slot.</param>
	/// <param name="Handler">Handler.</param>
	bool InteractionUpdate(HandApply interaction,InventorySlot Slot, ConstructionHandler Handler);

	/// <summary>
	/// Cans the interaction.
	/// </summary>
	/// <returns><c>true</c>, if interaction was caned, <c>false</c> otherwise.</returns>
	/// <param name="interaction">Interaction.</param>
	/// <param name="Slot">Slot.</param>
	/// <param name="Handler">Handler.</param>
	bool CanInteraction(HandApply interaction, InventorySlot Slot, ConstructionHandler Handler);
}
