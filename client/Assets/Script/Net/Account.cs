using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

namespace KBEngine
{
	public class Account : Entity
	{
		public override void __init__()
		{
			base.__init__();

			Debug.Log("Account is create.");

			Event.fireOut("AccountCreate", this);
		}
	}
}
