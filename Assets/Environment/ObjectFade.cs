using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFade : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask occluderLayers;

    [Header("Fade Material")]
    public Material fadeMaterial;

    [Header("Fade Settings")]
    public float fadedAlpha = 0.3f; // 30%
    public float fadeSpeed = 5f;

    private class OccluderData
    {
        public Renderer renderer;
        public Material[] originalMaterials;
        public Coroutine fadeRoutine;
    }

    private Dictionary<Renderer, OccluderData> activeOccluders =
        new Dictionary<Renderer, OccluderData>();

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(transform.position, direction);
        Debug.DrawRay(transform.position, direction, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray, distance, occluderLayers);
        HashSet<Renderer> hitRenderers = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponentInChildren<Renderer>();
            if (rend == null) continue;

            hitRenderers.Add(rend);

            if (!activeOccluders.ContainsKey(rend))
            {
                OccluderData data = new OccluderData();
                data.renderer = rend;
                data.originalMaterials = rend.materials;

                Material[] fadedMats = new Material[data.originalMaterials.Length];
                for (int i = 0; i < fadedMats.Length; i++)
                {
                    fadedMats[i] = new Material(fadeMaterial);
                }

                rend.materials = fadedMats;

                data.fadeRoutine = StartCoroutine(
                    FadeMaterials(rend.materials, 1f, fadedAlpha)
                );

                activeOccluders.Add(rend, data);
            }
        }

        // Restore objects no longer blocking the view
        List<Renderer> toRestore = new List<Renderer>();

        foreach (var pair in activeOccluders)
        {
            if (!hitRenderers.Contains(pair.Key))
                toRestore.Add(pair.Key);
        }

        foreach (Renderer rend in toRestore)
        {
            OccluderData data = activeOccluders[rend];

            if (data.fadeRoutine != null)
                StopCoroutine(data.fadeRoutine);

            data.fadeRoutine = StartCoroutine(
                FadeBackAndRestore(data)
            );

            activeOccluders.Remove(rend);
        }
    }

    IEnumerator FadeMaterials(Material[] mats, float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(from, to, t);

            foreach (Material mat in mats)
            {
                Color c = mat.color;
                c.a = a;
                mat.color = c;
            }

            yield return null;
        }
    }

    IEnumerator FadeBackAndRestore(OccluderData data)
    {
        Material[] mats = data.renderer.materials;
        float startAlpha = mats[0].color.a;
        float t = 0f;

        // Fade back to full opacity
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(startAlpha, 1f, t);

            foreach (Material mat in mats)
            {
                Color c = mat.color;
                c.a = a;
                mat.color = c;
            }

            yield return null;
        }

        // Restore original materials
        data.renderer.materials = data.originalMaterials;
    }
}
