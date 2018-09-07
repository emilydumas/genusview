// Painting an object involves copying (blitting) its texture to itself, but
// using a shader that adds a new spot.  However, this shader needs to know
// about the mapping between (u,v) coordinates in the paintable object and (u,v)
// coordinates in the texture.

// For the genus two surface, this mapping is the identity (uv on surface and uv
// in texture are the same).  For the H^2 view it is a more complicated mapping
// because each of the six basic tiles appears many times.

// Because of this, each paintable object needs its own painting shader that
// knows about the proper (u,v) mapping.

// This script decorates an object with another material which tells
// PaintableTexture which shader to use for this.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintData : MonoBehaviour {
	public Material paintMaterial = null;
}
