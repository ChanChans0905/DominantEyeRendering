Shader "Unlit/Screen_FFR"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // _Resolution_X("Resolution_X", Float) = 7680
        // _Resolution_Y("Resolution_Y", Float) = 4320
        _DownscaleFactor("Downscale Factor", Range(0,3)) = 1.0
        _GazePoint("Gaze Point", Vector) = (0,0,10,0)
        //_FoveaRegionRadius("Fovea Region Radius", Float) = 10
        //_GroupSize("Group Size", Range(0.01,1)) = 1.0
        _Sigma("Sigma", Range(0.1,5.0)) = 1.0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                
                // get world position of pixel
                float3 worldPos : TEXCOORD1;
            };

            // Variables
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _DownscaleFactor;
            float _Resolution_X;
            float _Resolution_Y; 
            float4 _GazePoint;
            float _FoveaRegionRadius;
            float _GroupSize;
            float _Sigma;
            float _GaussianRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

                        
            float Gaussian(float x, float y, float _Sigma) {
                float coeff = 1.0 / (2.0 * 3.14 * _Sigma * _Sigma);
                float expon = -(x*x + y*y) / (2.0 * _Sigma * _Sigma);
                return coeff * exp(expon);
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);
                float2 gridSize;

                float totalWeight = 0.0;
                int _GaussianRange=5;
                _Resolution_X = 7680.0;
                _Resolution_Y = 4320.0;

                // Fovea region : Sphere
                float DistanceVar = distance(i.worldPos, _GazePoint.xyz);

                float Distance_X = abs(i.worldPos.x - _GazePoint.x);
                float Distance_Y = abs(i.worldPos.y - _GazePoint.y);
                float Distance_Sum = Distance_X + Distance_Y;

                float Coor_X = i.worldPos.x;
                float Coor_Y = i.worldPos.y;
                float XXX = _GazePoint.x + _FoveaRegionRadius;
                float XXX2 = _GazePoint.x - _FoveaRegionRadius;
                float YYY = _GazePoint.y + _FoveaRegionRadius;
                float YYY2 = _GazePoint.y - _FoveaRegionRadius;


                // *********************** Multi Layer **************************

                // // //Circle Region
                // // bool FoveaRegion = DistanceVar < _FoveaRegionRadius;
                // // bool PeripheralRegion = DistanceVar > _FoveaRegionRadius + 2.0;
                // // bool BlendRegion_1 = DistanceVar > _FoveaRegionRadius + 0.0 && DistanceVar < _FoveaRegionRadius + 0.5;
                // // bool BlendRegion_2 = DistanceVar > _FoveaRegionRadius + 0.5 && DistanceVar < _FoveaRegionRadius + 1.0;
                // // bool BlendRegion_3 = DistanceVar > _FoveaRegionRadius + 1.0 && DistanceVar < _FoveaRegionRadius + 1.5;
                // // bool BlendRegion_4 = DistanceVar > _FoveaRegionRadius + 1.5 && DistanceVar < _FoveaRegionRadius + 2.0;

                // Square Region
                bool FoveaRegion = Coor_X < XXX && Coor_X > XXX2 && Coor_Y < YYY && Coor_Y > YYY2;
                bool BlendRegion_1 = Coor_X < XXX + 0.5 && Coor_X > XXX2 - 0.5 && Coor_Y < YYY + 0.5 && Coor_Y > YYY2 - 0.5; 
                bool BlendRegion_2 = Coor_X < XXX + 1.0 && Coor_X > XXX2 - 1.0 && Coor_Y < YYY + 1.0 && Coor_Y > YYY2 - 1.0; 
                bool BlendRegion_3 = Coor_X < XXX + 1.5 && Coor_X > XXX2 - 1.5 && Coor_Y < YYY + 1.5 && Coor_Y > YYY2 - 1.5; 
                bool BlendRegion_4 = Coor_X < XXX + 2.0 && Coor_X > XXX2 - 2.0 && Coor_Y < YYY + 2.0 && Coor_Y > YYY2 - 2.0; 

                if(FoveaRegion)
                {
                    col = tex2D(_MainTex,uv);
                }

                if(!FoveaRegion)
                {
                    if(BlendRegion_4)
                    {
                        gridSize = float2(_Resolution_X, _Resolution_Y);
                        gridSize *= _GroupSize*1.25;
                        //col = float4(1,1,1,1);
                        
                    }
                    if(BlendRegion_3)
                    {
                        gridSize = float2(_Resolution_X, _Resolution_Y);
                        gridSize *= _GroupSize*1.5;
                        //col = float4(0,1,1,1);
                    }
                    if(BlendRegion_2)
                    {
                        gridSize = float2(_Resolution_X, _Resolution_Y);
                        gridSize *= _GroupSize*1.75;
                        //col = float4(1,0,1,1);
                    }
                    if(BlendRegion_1)
                    {
                        gridSize = float2(_Resolution_X, _Resolution_Y);
                        gridSize *= _GroupSize*2.0;
                        //col = float4(1,1,0,1);
                    }

                    // Square Region
                    if(!BlendRegion_4) // Peripheral Region
                    {
                        gridSize = float2(_Resolution_X, _Resolution_Y);
                        gridSize *= _GroupSize;
                        //col = float4(1,0,0,1);
                    }

                    // // Circle Region
                    // if(PeripheralRegion)
                    // {
                    //     gridSize = float2(_Resolution_X, _Resolution_Y);
                    //     gridSize *= _GroupSize;
                    //     //col = float4(1,0,0,1);
                    // }

                    // // 원색 테스트
                    // return col;

                    float2 gridPosition = floor(i.uv * gridSize) / gridSize;
                    float2 gridStep = 1.0/gridSize;
                                                    
                    // 네 모서리 픽셀의 색상을 샘플링
                    float2 topLeft = gridPosition;
                    float2 topRight = gridPosition + float2(gridStep.x, 0);
                    float2 bottomLeft = gridPosition + float2(0, gridStep.y);
                    float2 bottomRight = gridPosition + gridStep;
    
                    float4 colorTopLeft = tex2D(_MainTex, topLeft);
                    float4 colorTopRight = tex2D(_MainTex, topRight);
                    float4 colorBottomLeft = tex2D(_MainTex, bottomLeft);
                    float4 colorBottomRight = tex2D(_MainTex, bottomRight);
                    
                    // 평균 색상 계산
                    float4 averageColor = (colorTopLeft + colorTopRight + colorBottomLeft + colorBottomRight) / 4.0;
                    
                    return averageColor;
                }



                                
                
                // *************** Gaussian Blur ******************

                // float2 GridUV = i.uv * float2(8.0, 8.0);
                // float2 GridPosition = floor(GridUV);
                // int GridNumber = int(GridPosition.y) * 8 + int(GridPosition.x) + 1;
                // bool FoveaRegion_Square = (GridNumber == 19 || GridNumber == 20 || GridNumber == 21 || GridNumber == 22
                //                         || GridNumber == 27 || GridNumber == 28 || GridNumber == 29 || GridNumber == 30
                //                         || GridNumber == 35 || GridNumber == 36 || GridNumber == 37 || GridNumber == 38
                //                         || GridNumber == 43 || GridNumber == 44 || GridNumber == 45 || GridNumber == 46);
                // bool GaussianRegion = (GridNumber == 10 || GridNumber == 11 || GridNumber == 12 || GridNumber == 13
                //                     || GridNumber == 14 || GridNumber == 15 || GridNumber == 18 || GridNumber == 23
                //                     || GridNumber == 26 || GridNumber == 31 || GridNumber == 34 || GridNumber == 39
                //                     || GridNumber == 42 || GridNumber == 47 || GridNumber == 50 || GridNumber == 51
                //                     || GridNumber == 52 || GridNumber == 53 || GridNumber == 54 || GridNumber == 55);
                // bool RestRegion = (GridNumber == 1 || GridNumber == 2 || GridNumber == 3 || GridNumber == 4
                //                 || GridNumber == 5 || GridNumber == 6 || GridNumber == 7 || GridNumber == 8
                //                 || GridNumber == 9 || GridNumber == 16 || GridNumber == 17 || GridNumber == 24
                //                 || GridNumber == 25 || GridNumber == 32 || GridNumber == 33 || GridNumber == 40
                //                 || GridNumber == 41 || GridNumber == 48 || GridNumber == 49 || GridNumber == 56
                //                 || GridNumber == 57 || GridNumber == 58 || GridNumber == 59 || GridNumber == 60
                //                 || GridNumber == 61 || GridNumber == 62 || GridNumber == 63 || GridNumber == 64);

                // if(GaussianRegion)
                // {
                //     for (int x = -_GaussianRange; x <= _GaussianRange; x++) {
                //         for (int y = -_GaussianRange; y <= _GaussianRange; y++) {
                //             float2 offset = float2(x, y) / float2(3840.0, 2160.0) ;
                //             float weight = Gaussian(x, y , _Sigma);
                //             col += tex2D(_MainTex, uv + offset) * weight;
                //             totalWeight += weight;
                //         }
                //     }
                //     col /= totalWeight; // 정규화를 통한 최종 색상 계산 
                // }
                                
                // if(RestRegion){
                // //     // uv.x *= _Resolution_X * _DownscaleFactor;
                // //     // uv.x = floor(uv.x) / _Resolution_X / _DownscaleFactor;
                // //     // uv.y *= _Resolution_Y * _DownscaleFactor;
                // //     // uv.y = floor(uv.y) / _Resolution_Y / _DownscaleFactor;

                // // 정의된 영역에 따라 UV 좌표를 조정
                // float2 gridSize = float2(3840.0, 2160.0);
                // gridSize *= _GroupSize;
                // float2 gridPosition = floor(i.uv * gridSize) / gridSize;
                // float2 gridStep = 1.0/gridSize;

                // // 네 모서리 픽셀의 색상을 샘플링
                // float2 topLeft = gridPosition;
                // float2 topRight = gridPosition + float2(gridStep.x, 0);
                // float2 bottomLeft = gridPosition + float2(0, gridStep.y);
                // float2 bottomRight = gridPosition + gridStep;

                // float4 colorTopLeft = tex2D(_MainTex, topLeft);
                // float4 colorTopRight = tex2D(_MainTex, topRight);
                // float4 colorBottomLeft = tex2D(_MainTex, bottomLeft);
                // float4 colorBottomRight = tex2D(_MainTex, bottomRight);
                
                // // 평균 색상 계산
                // float4 averageColor = (colorTopLeft + colorTopRight + colorBottomLeft + colorBottomRight) / 4.0;
                
                // return averageColor;
                // }

                // if(FoveaRegion_Square)
                // {
                //     col = tex2D(_MainTex,uv);
                // }

                return col;
            }
            ENDCG
        }
    }
}
