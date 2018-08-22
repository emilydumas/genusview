struct vect_in_fund {
    // vector in square fund domain
    float3 v;
    // which s3 coset is the original point in?
    uint coset;
};

// Spacelike vectors giving the sides of a square tile
static float3 sides[4] = {
    float3(-1.22474487139, 0, 0.707106781187), 
    float3(0, -1.22474487139, 0.707106781187),
    float3(1.22474487139, 0, 0.707106781187),
    float3(0, 1.22474487139, 0.707106781187)
    };
  
// Minkowski reflections in the sides
static float3x3 reflections[4] = {
    float3x3(-2.00000000000, 0, -1.73205080757,
             0, 1.00000000000, 0,
            1.73205080757, 0, 2.00000000000),
    float3x3(1.00000000000, 0, 0,
             0, -2.00000000000, -1.73205080757,
             0, 1.73205080757, 2.00000000000),
    float3x3(-2.00000000000, 0, 1.73205080757,
             0, 1.00000000000, 0,
            -1.73205080757, 0, 2.00000000000),
    float3x3(1.00000000000, 0, 0,
            0, -2.00000000000, 1.73205080757,
            0, -1.73205080757, 2.00000000000)
};

static uint perms[4][6] = { {1,0,5,4,3,2}, {5,4,3,2,1,0}, {1,0,5,4,3,2}, {5,4,3,2,1,0}};

static uint s3inv[6] = {0,1, 4, 3, 2, 5};

static const float3 signature = float3(-1,-1,1);

float minkdot(float3 v, float3 w)
{
    return dot(v,signature*w);
}


float3 fromklein(float2 xy)
{
    float t = rsqrt(1.0 - xy.x*xy.x - xy.y*xy.y);
    return float3(t*xy.x,t*xy.y,t);
}

float2 toklein(float3 v)
{
    return float2(v.x/v.z, v.y/v.z);
}

vect_in_fund tofund(float3 v0)
{
    vect_in_fund ret;
    int i=0;

    ret.v = v0;
    ret.coset = 0;

    while (i<10) {
        int k;
        for (k=0;k<4;k++) {
            if (minkdot(ret.v,sides[k]) < 0) {
                ret.v = mul(reflections[k],ret.v);
                ret.coset = perms[k][ret.coset];
                break;
            }
        }
        if (k == 4) {
            // No reflections were applied. Success.
            // But the coset label is inverted; fix that.
            ret.coset = s3inv[ret.coset];
            return ret;
        }
        i++;
    }
    // Reached max iterations without being in fund domain.
    // Return error sentinel.
    ret.coset = 255;
    return ret;
}

float2 six_panel_vif_to_uv(vect_in_fund vf)
{
    // Retrieve the color from one of six panels inside a single texture
    // MAJOR GOTCHA: The texture must NOT have mipmaps enabled!
    float2 xy = toklein(vf.v);
    float2 uv0 = 0.3333333333333*0.5*(xy + 1.0);
    uv0 = uv0 + float2(0.3333333333333 * (vf.coset%3), 0.3333333333333 * (vf.coset/3));
    return uv0;
}

vect_in_fund six_panel_uv_to_vif(float2 uv)
{
    vect_in_fund vf;
    uint coset;
    uint i,j;

    // which panel are we in?
    i = floor(3*uv.x);
    j = floor(3*uv.y);
    coset = i+3*j;

    // and what position within the panel?
    uv = uv - float2(0.3333333333333 * i, 0.3333333333333 * j);
    float2 xy = 6*uv - 1;

    if (length(xy) < 1) {
        vf.v = fromklein(xy);
        vf.coset = coset;
    } else {
        vf.v = float3(0,0,0);
        vf.coset = 255;
    }
    return vf;
}

float hyp_quasi_dist(float3 v, float3 w)
{
    return minkdot(v,w)-1.0;
}