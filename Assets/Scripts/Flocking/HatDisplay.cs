using Avrahamy.EditorGadgets;
using Avrahamy.Meshes;
using BitStrap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Flocking
{
    public class HatDisplay : MonoBehaviour
    {
        [SerializeField]
        private EditableMesh mesh;

        [SerializeField]
        private MeshCollider meshCollider;

        [Inline]
        [SerializeField]
        private HatGenerator generator;
        [SerializeField] int currentSeed;
        [Info("Copy Current Seed here to get the same result. Useful for debugging")]
        [SerializeField] int nextSeed;

        protected void Awake()
        {
            Generate();
        }

        [Button]
        private void Generate()
        {
            currentSeed = nextSeed;
            generator.Generate(mesh);
            meshCollider.sharedMesh = mesh.Mesh;

            nextSeed = Random.Range(0, int.MaxValue);
        }
    }
}