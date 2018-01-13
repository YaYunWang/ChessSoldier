using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public class Avatar : AvatarBase
{
	public override void __init__()
	{
		base.__init__();

		Debug.Log("avatar create.");
	}
}
