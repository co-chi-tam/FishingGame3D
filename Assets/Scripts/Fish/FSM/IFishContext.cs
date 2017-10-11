using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public interface IFishContext : IContext {

	bool HaveBait();
	bool IsBiteBail();
	bool IsBailTimer();

}
