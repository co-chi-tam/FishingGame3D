using System;
using FSM;

public interface IFishContext : IContext {

	bool HaveBait();
	bool IsBiteBail();
	bool IsBailTimer();

}
