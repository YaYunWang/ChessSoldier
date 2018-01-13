using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

namespace KBEngine
{
	public class Account : AccountBase
	{
		public override void __init__()
		{
			base.__init__();

			Event.fireOut("AccountCreate", this);
		}

		public override void ReCreateAccountResponse(int arg1)
		{
			Event.fireOut("ReCreateAccountResponse", arg1);
		}

		public override void QueryPlayerCountResponse(uint arg1)
		{
			Debug.Log("当前人数：" + arg1);
		}

		public override void onInitBattleField()
		{
			Event.fireOut("StartBattleFieldEvent");
		}

		public override void onMarchMsg(string arg1)
		{
			Debug.Log("march msg:" + arg1);
		}
	}
}
