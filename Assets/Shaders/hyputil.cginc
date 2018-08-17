struct vect_in_fund {
    // vector in square fund domain
    float3 v;
    // which s3 coset is the original point in?
    int coset;
};

const float3 sides[4] = { {-1.22474, 0., 0.707107},
    {0,-1.22474, 0.707107},
    {1.22474, 0., 0.707107},
    {0,1.22474, 0.707107}
};

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

    ret.v = v0;
    ret.coset = 0;
    return ret;
}

float2 six_panel_vif_to_uv(vect_in_fund vf)
{
    float2 xy = toklein(vf.v);
    float2 uv0 = 0.3333333333333*0.5*(xy + 1.0);
    uv0 = uv0 + float2(0.3333333333333 * (vf.coset%3), 0.3333333333333 * (vf.coset/3));
    return uv0*float2(1,-1) + float2(0,1);
}