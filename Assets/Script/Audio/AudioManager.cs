using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance { get; set; }

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip BackGroundSound;
    public AudioClip DoorSFX;
    public AudioClip PC_SFX;
    public AudioClip Bed_SFX;
    public AudioClip Setting_SFX;
    public AudioClip Click_SFX;


    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource.clip = BackGroundSound;
        musicSource.Play();
    }

    public void playSFX(AudioClip clip, float pitch = 1.0f, bool useRandomPitch = false, float minPitch = 1.0f, float maxPitch = 1.2f)
{
    if (SFXSource != null && clip != null)
    {
        float finalPitch = useRandomPitch ? Random.Range(minPitch, maxPitch) : pitch;
        SFXSource.pitch = finalPitch;
        SFXSource.PlayOneShot(clip);
    }
}
}