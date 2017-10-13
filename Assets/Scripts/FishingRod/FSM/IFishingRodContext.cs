using System;
using FSM;

public interface IFishingRodContext : IContext {

	bool IsBaitThrowed ();
	bool IsFishBite ();
	bool IsFishingEnd ();

}
