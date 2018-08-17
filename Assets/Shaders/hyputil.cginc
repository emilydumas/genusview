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
        if (minkdot(ret.v,sides[0]) <= 0)
        {
            ret.v = mul(reflections[0],ret.v);
            ret.coset = perms[0][ret.coset];
        } else {
            ret.coset = s3inv[ret.coset];
            break;
        }
        i++;
    }
    return ret;
}

float2 six_panel_vif_to_uv(vect_in_fund vf)
{
    float2 xy = toklein(vf.v);
    float2 uv0 = 0.3333333333333*0.5*(xy + 1.0);
    uv0 = uv0 + float2(0.3333333333333 * (vf.coset%3), 0.3333333333333 * (vf.coset/3));
    return uv0*float2(1,-1) + float2(0,1);
}