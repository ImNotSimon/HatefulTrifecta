using UnityEngine;
using System.Threading.Tasks;

public class ParticleRenderingRange : MonoBehaviour
{
    public float maxRenderDistance = 50f; // Maximum distance to render the particle system
    private ParticleSystem ps;
    private Transform cameraTransform;
    private bool isCheckingDistance; // Prevent multiple async tasks

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        cameraTransform = Camera.main.transform; // Replace with a specific camera if needed
        isCheckingDistance = false;
        CheckDistanceAsync(); // Start the async loop
    }

    private async void CheckDistanceAsync()
    {
        isCheckingDistance = true;
        while (isCheckingDistance)
        {
            await Task.Delay(100); // Adjust delay for performance (100ms = 10 checks per second)

            float distance = Vector3.Distance(transform.position, cameraTransform.position);

            if (distance <= maxRenderDistance)
            {
                EnableParticles(); // Ensure the particle system is active
            }
            else
            {
                DisableParticles(); // Stop emitting particles but reset if necessary
            }
        }
    }

    private void EnableParticles()
    {
        var emission = ps.emission;
        emission.enabled = true;

        if (!ps.isPlaying)
        {
            ps.Play(); // Start playing particles if not already playing
        }
    }

    private void DisableParticles()
    {
        var emission = ps.emission;
        emission.enabled = false; // Disable emission without stopping the system

        if (ps.isPlaying)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            // Stop emitting particles but allow existing particles to complete their lifecycle
        }
    }

    private void OnDestroy()
    {
        isCheckingDistance = false; // Stop the async loop
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the render range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRenderDistance);
    }
}
