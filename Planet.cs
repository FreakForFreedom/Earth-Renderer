﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Device22
{
    public class Planet
    {
        Terrain[] planetFace;
        QuadTree[] planetFaceQT;
        QuadTreeNode mainNode;

        private int terrainSize = 1025;
        private int patchSize = 513;
        private float planetRadius = 100.0f;
        private int counter = 6;
        private int maxLOD = 6;

        public Planet()
        {
            int tex0 = Texture.loadFromFile("earthcubemap_tex_c04.bmp");
            int tex1 = Texture.loadFromFile("earthcubemap_tex_c02.bmp");
            int tex2 = Texture.loadFromFile("earthcubemap_tex_c05.bmp");
            int tex3 = Texture.loadFromFile("earthcubemap_tex_c03.bmp");
            int tex4 = Texture.loadFromFile("earthcubemap_tex_c00.bmp");
            int tex5 = Texture.loadFromFile("earthcubemap_tex_c01.bmp");

            planetFace = new Terrain[counter];
            planetFaceQT = new QuadTree[counter];
            
            // The number of patches MUST be divisible by pow 4.

            // Left face:
            planetFace[0] = new Terrain(new Vector3(-(terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex0,
                                        "earthcubemap_c04.jpg");

            // Top face:
            planetFace[1] = new Terrain(new Vector3(-(terrainSize / 2), (terrainSize / 2) - 1, -(terrainSize / 2)),
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex1,
                                        "earthcubemap_c02.jpg");

            // Right face:
            planetFace[2] = new Terrain(new Vector3((terrainSize / 2) - 1, -(terrainSize / 2) - 1, (terrainSize / 2) - 1),
                                        new Vector3(-1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex2,
                                        "earthcubemap_c05.jpg");

            // Bottom face:
            planetFace[3] = new Terrain(new Vector3((terrainSize / 2) - 1, -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(-1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex3,
                                        "earthcubemap_c03.jpg");
            planetFaceQT[3] = new QuadTree(ref planetFace[3]);

            // Front face:
            planetFace[4] = new Terrain(new Vector3(-(terrainSize / 2), (terrainSize / 2) - 1, -(terrainSize / 2)),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex4,
                                        "earthcubemap_c00.jpg");

            // Back face:
            planetFace[5] = new Terrain(new Vector3((terrainSize / 2) - 1, -(terrainSize / 2), -(terrainSize / 2)),
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        maxLOD,
                                        terrainSize,
                                        patchSize,
                                        planetRadius,
                                        tex5,
                                        "earthcubemap_c01.jpg");

            // Generate the first node and assign manually the boundingbox.
            mainNode = new QuadTreeNode(0, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            mainNode.BoundingBox3D[0] = new Vector3(-(terrainSize / 2), -(terrainSize / 2), (terrainSize / 2));
            mainNode.BoundingBox3D[1] = new Vector3((terrainSize / 2), -(terrainSize / 2), (terrainSize / 2));
            mainNode.BoundingBox3D[2] = new Vector3(-(terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2));
            mainNode.BoundingBox3D[3] = new Vector3((terrainSize / 2), -(terrainSize / 2), -(terrainSize / 2));
            mainNode.BoundingBox3D[4] = new Vector3(-(terrainSize / 2), (terrainSize / 2), (terrainSize / 2));
            mainNode.BoundingBox3D[5] = new Vector3((terrainSize / 2), (terrainSize / 2), (terrainSize / 2));
            mainNode.BoundingBox3D[6] = new Vector3(-(terrainSize / 2), (terrainSize / 2), -(terrainSize / 2));
            mainNode.BoundingBox3D[7] = new Vector3((terrainSize / 2), (terrainSize / 2), -(terrainSize / 2));
            // Since its the first node, it gets the first id.
            mainNode.ID = -1;
            Debug.Trace("BB: Level " + -1 + " -> Generated");
            
            // Calculate and assign the nodes neighbors.
            for (int i = 0; i < counter; i++)
            {
                planetFaceQT[i] = new QuadTree(ref planetFace[i]);
                planetFaceQT[i].MainNode.Parent = mainNode;
                //QuadTreeNode mNode = planetFaceQT[i].MainNode;
                //QuadTree.CalculateQuadtreeNeighbors(ref mNode);
            }

            // Since there are six terrain faces to create the sphere, there is need to assign the missing border neighbors manually.
            // Neighbors for the left face:
            planetFaceQT[0].MainNode.Neighbor_N = planetFaceQT[1].MainNode;
            planetFaceQT[0].MainNode.Neighbor_E = planetFaceQT[5].MainNode;
            planetFaceQT[0].MainNode.Neighbor_S = planetFaceQT[3].MainNode;
            planetFaceQT[0].MainNode.Neighbor_W = planetFaceQT[4].MainNode;

            // Neighbors for the top face:
            planetFaceQT[1].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[1].MainNode.Neighbor_E = planetFaceQT[5].MainNode;
            planetFaceQT[1].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[1].MainNode.Neighbor_W = planetFaceQT[4].MainNode;

            // Neighbors for the right face:
            planetFaceQT[2].MainNode.Neighbor_N = planetFaceQT[3].MainNode;
            planetFaceQT[2].MainNode.Neighbor_E = planetFaceQT[4].MainNode;
            planetFaceQT[2].MainNode.Neighbor_S = planetFaceQT[1].MainNode;
            planetFaceQT[2].MainNode.Neighbor_W = planetFaceQT[5].MainNode;

            // Neighbors for the bottom face:
            planetFaceQT[3].MainNode.Neighbor_N = planetFaceQT[0].MainNode;
            planetFaceQT[3].MainNode.Neighbor_E = planetFaceQT[4].MainNode;
            planetFaceQT[3].MainNode.Neighbor_S = planetFaceQT[2].MainNode;
            planetFaceQT[3].MainNode.Neighbor_W = planetFaceQT[5].MainNode;

            // Neighbors for the front face:
            planetFaceQT[4].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[4].MainNode.Neighbor_E = planetFaceQT[1].MainNode;
            planetFaceQT[4].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[4].MainNode.Neighbor_W = planetFaceQT[3].MainNode;

            // Neighbors for the back face:
            planetFaceQT[5].MainNode.Neighbor_N = planetFaceQT[2].MainNode;
            planetFaceQT[5].MainNode.Neighbor_E = planetFaceQT[3].MainNode;
            planetFaceQT[5].MainNode.Neighbor_S = planetFaceQT[0].MainNode;
            planetFaceQT[5].MainNode.Neighbor_W = planetFaceQT[1].MainNode;


            // Calculate and assign the missing nodes neighbors.
            for (int i = 0; i < counter; i++)
            {
                QuadTreeNode mNode = planetFaceQT[i].MainNode;
                QuadTree.CalculateQuadtreeNeighbors(ref mNode);
            }
            
            Debug.Trace("Planet generated!");
        }

        public void Render(ref Frustum frustum)
        {
            for (int i = 0; i < planetFaceQT.Length; i++)
            {
               if (planetFaceQT[i] != null) planetFaceQT[i].Render(ref frustum);
            }
        }

        public void Destroy()
        {

        }
    }
}
