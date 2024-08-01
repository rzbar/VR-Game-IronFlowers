using System;
using UnityEngine;

namespace MeshFactory
{
    public class MeshFactory
    {
        /// <summary>
        /// return a mesh of wireframe sphere
        /// </summary>
        /// <param name="numOfMeridian">经线的个数</param>
        /// <param name="numOfParallel">纬线的个数</param>
        /// <returns></returns>
        public static Mesh WireframeSphere(int numOfMeridian, int numOfParallel)
        {
            //当经纬线数量不足以构成球体时，警告并返回空值
            if (numOfMeridian < 1 || numOfParallel < 1)
            {
                Debug.LogWarning("MeshFactory.cs: the arguments of sphere is wrong.");
                return null;
            }

            //创建最后需要返回值
            Mesh mesh = new Mesh();
            
            //创建顶点数组
            Vector3[] verts = new Vector3[numOfMeridian * numOfParallel * 2 + 2];
            //创建网格面索引数组（用于创建网格）
            //此处需要连出线框，所以相邻两点间要有连线。一个线有两个点，所以数组大小为线条数 * 2
            //数组大小 = (横线数量 + 纵线数量) * 2
            //       = (经线个数 * 纬线个数 + 经线个数 * (纬线个数 + 1)) * 2
            int[] indices = new int[numOfMeridian * (numOfParallel * 2 + 1) * 2];
            
            //保存每个经度所对应的sin、cos值数组
            float[] sinsLongtitude = new float[numOfMeridian];
            float[] cossLongtitude = new float[numOfMeridian];
            {
                float angleIncrement = 2 * Mathf.PI / numOfMeridian;
                for (int i = 0; i < numOfMeridian; i++)
                {
                    sinsLongtitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLongtitude[i] = (float)Math.Cos(angleIncrement * i);
                }
            }

            //保存每个纬度所对应的sin、cos值数组
            float[] sinsLatitude = new float[numOfParallel];
            float[] cossLatitude = new float[numOfParallel];
            {
                float angleIncrement = Mathf.PI / numOfParallel;
                for (int i = 0; i < numOfParallel; i++)
                {
                    sinsLatitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLatitude[i] = (float)Math.Cos(angleIncrement * i);
                }
            }

            //创建顶底点指针，并在顶点数组中赋值
            int topIndex = verts.Length - 2;
            int bottomIndex = verts.Length - 1;
            verts[topIndex] = new Vector3(0.0f, 1.0f, 0.0f);
            verts[bottomIndex] = new Vector3(0.0f, -1.0f, 0.0f);
            
            //遍历每一个顶点，并赋值和连线
            int vertsIndex = 0;
            int indicesIndex = 0;
            for (int i = 0; i < numOfMeridian; i++)
            {
                float sinLongtitude = sinsLongtitude[i];
                float cosLongtitude = cossLongtitude[i];
                for (int j = 0; j < numOfParallel; j++, vertsIndex++)
                {
                    float sinLatitude = sinsLatitude[j];
                    float cosLatitude = cossLatitude[j];
                    
                    verts[vertsIndex] = 
                        new Vector3(cosLongtitude * sinLatitude, cosLatitude, sinLongtitude * sinLatitude);
                    
                    //向上连接纵线
                    indices[indicesIndex++] = i == 0 ? topIndex : vertsIndex - 1;
                    indices[indicesIndex++] = vertsIndex;
                    //向一侧连接横线
                    indices[indicesIndex++] = vertsIndex;
                    indices[indicesIndex++] = (vertsIndex + numOfParallel) % (2 * numOfMeridian * numOfParallel + 2);
                    //如果该点下面为底点，向下连接
                    if (j == numOfParallel - 1)
                    {
                        indices[indicesIndex++] = vertsIndex;
                        indices[indicesIndex++] = bottomIndex;
                    }
                }
            }

            mesh.vertices = verts;
            mesh.normals = verts;
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            
            return mesh;
        }

        /// <summary>
        /// <para>return a mesh of solid color sphere</para>
        /// <para>返回一个纯色球体的网格</para>
        /// </summary>
        /// <param name="numOfMeridian">经线的个数</param>
        /// <param name="numOfParallel">纬线的个数</param>
        /// <returns></returns>
        public static Mesh SolidColorSphere(int numOfMeridian, int numOfParallel)
        {
            //当经纬线数量不足以构成球体时，警告并返回空值
            if (numOfMeridian < 1 || numOfParallel < 1)
            {
                Debug.LogWarning("MeshFactory.cs: the arguments of sphere is wrong.");
                return null;
            }

            //创建最后需要返回值
            Mesh mesh = new Mesh();
            
            //创建顶点数组
            Vector3[] verts = new Vector3[numOfMeridian * numOfParallel + 2];
            //创建网格索引数组
            //数组大小 = 三角形数 * 3 = (原四边形数 * 2 + 原三边形数) * 3 = (经线数 * (纬线数 - 1) * 2 + 经线数 * 2) * 3
            int[] indices = new int[numOfMeridian * numOfParallel * 6];
            
            //创建各经度对应sin、cos值数组
            float[] sinsLongtitude = new float[numOfMeridian];
            float[] cossLongtitude = new float[numOfMeridian];
            {
                float angleIncrement = 2 * Mathf.PI / numOfMeridian;
                for (int i = 0; i < numOfMeridian; i++)
                {
                    sinsLongtitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLongtitude[i] = (float)Mathf.Cos(angleIncrement * i);
                }
            }
            
            //创建各纬度对应的sin、cos数组
            float[] sinsLatitude = new float[numOfParallel];
            float[] cossLatitude = new float[numOfParallel];
            {
                float angleIncrement = Mathf.PI / numOfParallel;
                for (int i = 0; i < numOfParallel; i++)
                {
                    sinsLatitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLatitude[i] = (float)Math.Cos(angleIncrement * i);
                }
            }

            int topIndex = verts.Length - 2;
            int bottomIndex = verts.Length - 1;
            verts[topIndex] = new Vector3(0.0f, 1.0f, 0.0f);
            verts[bottomIndex] = new Vector3(0.0f, -1.0f, 0.0f);

            int vertsIndex = 0;
            int indicesIndex = 0;
            for (int i = 0; i < numOfMeridian; i++)
            {
                float sinLongtitude = sinsLongtitude[i];
                float cosLongtitude = cossLongtitude[i];
                for (int j = 0; j < numOfParallel; j++, vertsIndex++)
                {
                    float sinLatitude = sinsLatitude[j];
                    float cosLatitude = cossLatitude[j];

                    verts[vertsIndex] = new Vector3(cosLongtitude * sinLatitude, cosLatitude,
                        sinLongtitude * sinLatitude);
                    
                    //下一个经线上同位置的点
                    int vertsIndexOnNextMeridian = (vertsIndex + numOfParallel) % (numOfMeridian * numOfParallel);
                    //下一个经线上上一个位置的点
                    int preVertsIndexOnNextMeridian = (vertsIndex + numOfParallel - 1) % (numOfMeridian * numOfParallel);

                    if (j == 0)
                    {
                        indices[indicesIndex++] = topIndex;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndex;
                    }
                    else
                    {
                        indices[indicesIndex++] = vertsIndex - 1;
                        indices[indicesIndex++] = preVertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndex;

                        indices[indicesIndex++] = vertsIndex;
                        indices[indicesIndex++] = preVertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                    }
                    if (j == numOfParallel - 1)
                    {
                        indices[indicesIndex++] = vertsIndex;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                        indices[indicesIndex++] = bottomIndex;
                    }
                }
            }

            mesh.vertices = verts;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            return mesh;
        }
        
        /// <summary>
        /// <para>return a mesh of smooth shaded sphere</para>
        /// <para>返回一个平滑球体的网格</para>
        /// </summary>
        /// <param name="numOfMeridian">经线的个数</param>
        /// <param name="numOfParallel">纬线的个数</param>
        /// <returns></returns>
        public static Mesh SmoothShadedSphere(int numOfMeridian, int numOfParallel)
        {
            //当经纬线数量不足以构成球体时，警告并返回空值
            if (numOfMeridian < 1 || numOfParallel < 1)
            {
                Debug.LogWarning("MeshFactory.cs: the arguments of sphere is wrong.");
                return null;
            }

            //创建最后需要返回值
            Mesh mesh = new Mesh();
            
            //创建顶点数组
            Vector3[] verts = new Vector3[numOfMeridian * numOfParallel + 2];
            //创建网格索引数组
            //数组大小 = 三角形数 * 3 = (原四边形数 * 2 + 原三边形数) * 3 = (经线数 * (纬线数 - 1) * 2 + 经线数 * 2) * 3
            int[] indices = new int[numOfMeridian * numOfParallel * 6];
            //创建法线数组
            Vector3[] normals = new Vector3[verts.Length];
            
            //创建各经度对应sin、cos值数组
            float[] sinsLongtitude = new float[numOfMeridian];
            float[] cossLongtitude = new float[numOfMeridian];
            {
                float angleIncrement = 2 * Mathf.PI / numOfMeridian;
                for (int i = 0; i < numOfMeridian; i++)
                {
                    sinsLongtitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLongtitude[i] = (float)Mathf.Cos(angleIncrement * i);
                }
            }
            
            //创建各纬度对应的sin、cos数组
            float[] sinsLatitude = new float[numOfParallel];
            float[] cossLatitude = new float[numOfParallel];
            {
                float angleIncrement = Mathf.PI / numOfParallel;
                for (int i = 0; i < numOfParallel; i++)
                {
                    sinsLatitude[i] = (float)Math.Sin(angleIncrement * i);
                    cossLatitude[i] = (float)Math.Cos(angleIncrement * i);
                }
            }

            int topIndex = verts.Length - 2;
            int bottomIndex = verts.Length - 1;
            verts[topIndex] = new Vector3(0.0f, 1.0f, 0.0f);
            verts[bottomIndex] = new Vector3(0.0f, -1.0f, 0.0f);
            normals[topIndex] = verts[topIndex];
            normals[bottomIndex] = verts[bottomIndex];

            int vertsIndex = 0;
            int indicesIndex = 0;
            for (int i = 0; i < numOfMeridian; i++)
            {
                float sinLongtitude = sinsLongtitude[i];
                float cosLongtitude = cossLongtitude[i];
                for (int j = 0; j < numOfParallel; j++, vertsIndex++)
                {
                    float sinLatitude = sinsLatitude[j];
                    float cosLatitude = cossLatitude[j];

                    verts[vertsIndex] = new Vector3(cosLongtitude * sinLatitude, cosLatitude,
                        sinLongtitude * sinLatitude);
                    normals[vertsIndex] = verts[vertsIndex];
                    
                    //下一个经线上同位置的点
                    int vertsIndexOnNextMeridian = (vertsIndex + numOfParallel) % (numOfMeridian * numOfParallel);
                    //下一个经线上上一个位置的点
                    int preVertsIndexOnNextMeridian = (vertsIndex + numOfParallel - 1) % (numOfMeridian * numOfParallel);

                    if (j == 0)
                    {
                        indices[indicesIndex++] = topIndex;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndex;
                    }
                    else
                    {
                        indices[indicesIndex++] = vertsIndex - 1;
                        indices[indicesIndex++] = preVertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndex;

                        indices[indicesIndex++] = vertsIndex;
                        indices[indicesIndex++] = preVertsIndexOnNextMeridian;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                    }
                    if (j == numOfParallel - 1)
                    {
                        indices[indicesIndex++] = vertsIndex;
                        indices[indicesIndex++] = vertsIndexOnNextMeridian;
                        indices[indicesIndex++] = bottomIndex;
                    }
                }
            }

            mesh.vertices = verts;
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.normals = normals;

            return mesh;
        }
    }
}