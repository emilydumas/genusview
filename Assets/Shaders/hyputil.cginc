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

// Elements of the Fuchsian group.  First four are the reflections
// in the edge vectors given above (in the same order!)
static float3x3 fuchs[17] = {
    // R1
    float3x3(-2.0,0.0,-1.7320508075688772,
             0.0,1.0,0.0,
             1.7320508075688772,0.0,2.0),
    // R2
    float3x3(1.0,0.0,0.0,
             0.0,-2.0,-1.7320508075688772,
             0.0,1.7320508075688772,2.0),
    // R3
    float3x3(-2.0,0.0,1.7320508075688772,
             0.0,1.0,0.0,
             -1.7320508075688772,0.0,2.0),
    // R4
    float3x3(1.0,0.0,0.0,
             0.0,-2.0,1.7320508075688772,
             0.0,-1.7320508075688772,2.0),
    // R34
    float3x3(-2.0,-3.0,3.4641016151377544,
             0.0,-2.0,1.7320508075688772,
             -1.7320508075688772,-3.4641016151377544,4.0),
    // R32
    float3x3(-2.0,3.0,3.4641016151377544,
             0.0,-2.0,-1.7320508075688772,
             -1.7320508075688772,3.4641016151377544,4.0),
    // R12
    float3x3(-2.0,-3.0,-3.4641016151377544,
             0.0,-2.0,-1.7320508075688772,
             1.7320508075688772,3.4641016151377544,4.0),
    // R14
    float3x3(-2.0,3.0,-3.4641016151377544,
             0.0,-2.0,1.7320508075688772,
             1.7320508075688772,-3.4641016151377544,4.0),
    // R43
    float3x3(-2.0,0.0,1.7320508075688772,
             -3.0,-2.0,3.4641016151377544,
             -3.4641016151377544,-1.7320508075688772,4.0),
    // R23
    float3x3(-2.0,0.0,1.7320508075688772,
             3.0,-2.0,-3.4641016151377544,
             -3.4641016151377544,1.7320508075688772,4.0),
    // R21
    float3x3(-2.0,0.0,-1.7320508075688772,
             -3.0,-2.0,-3.4641016151377544,
             3.4641016151377544,1.7320508075688772,4.0),
    // R41
    float3x3(-2.0,0.0,-1.7320508075688772,
             3.0,-2.0,3.4641016151377544,
             3.4641016151377544,-1.7320508075688772,4.0),
    // R343
    float3x3(-2.0,-3.0,3.4641016151377544,
             -3.0,-2.0,3.4641016151377544,
             -3.4641016151377544,-3.4641016151377544,5.0),
    // R232
    float3x3(-2.0,3.0,3.4641016151377544,
             3.0,-2.0,-3.4641016151377544,
             -3.4641016151377544,3.4641016151377544,5.0),
    // R121
    float3x3(-2.0,-3.0,-3.4641016151377544,
             -3.0,-2.0,-3.4641016151377544,
             3.4641016151377544,3.4641016151377544,5.0),
    // R414
    float3x3(-2.0,3.0,-3.4641016151377544,
             3.0,-2.0,3.4641016151377544,
             3.4641016151377544,-3.4641016151377544,5.0),
    // identity
    float3x3(1.0,0.0,0.0,
             0.0,1.0,0.0,
             0.0,0.0,1.0)
};

// We enumerate the elements of S3 as follows
// 0         1         2         3         4         5
// e         (1,2)     (1,2,3)   (2,3)     (3,2,1)   (1,3)

// Table of left multiplication by {1,3,1,3}
static uint perms[4][6] = { {1,0,5,4,3,2}, {5,4,3,2,1,0}, {1,0,5,4,3,2}, {5,4,3,2,1,0}};

// Table of inverses in S3
static uint s3inv[6] = {0,1, 4, 3, 2, 5};

// Tables of ratios; s3ratio[i][j] is  i * inverse(j)
static uint s3ratio[6][6] = {
    {0,1,4,3,2,5},
    {1,0,3,4,5,2},
    {2,3,0,5,4,1},
    {3,2,5,0,1,4},
    {4,5,2,1,0,3},
    {5,4,1,2,3,0}
};

// Suppose two points a,b are in tiles with relative position i
// Then to find the closest equivalent pair, we should apply some
// Fuchsian group elements to b.  The indices in fuchs[] to use are given by
// possible_closest[i].
static uint possible_closest[6][4] = {
    {16,16,16,16},
    {0,0,2,2},
    {4,5,6,7},
    {12,13,14,15},
    {8,9,10,11},
    {1,1,3,3}
};


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
                ret.v = mul(fuchs[k],ret.v);
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

float g2_quasi_dist(vect_in_fund a, vect_in_fund b)
{
    uint i = s3ratio[s3inv[b.coset]][s3inv[a.coset]];
    float d0 = hyp_quasi_dist(b.v,mul(fuchs[possible_closest[i][0]],a.v));
    float d1 = hyp_quasi_dist(b.v,mul(fuchs[possible_closest[i][1]],a.v));
    float d2 = hyp_quasi_dist(b.v,mul(fuchs[possible_closest[i][2]],a.v));
    float d3 = hyp_quasi_dist(b.v,mul(fuchs[possible_closest[i][3]],a.v));
    return min(min(d0,d1),min(d2,d3));
}