using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to the Powerup to indicate how much points it will add to the score, not used in the final build
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class PowerupPointValue : MonoBehaviour {
	private long points = 100;  ///< The number of points added to the score, can be changed in different prefab instances

    /// <summary>
    /// Return the points variable
    /// </summary>
	public long getPoints(){
		return points;
	}
}
