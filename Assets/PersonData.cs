using UnityEngine;

[CreateAssetMenu(fileName = "PersonData", menuName = "Arctic/PersonData")]
public class PersonData : ScriptableObject
{
    [SerializeField] private float _walkingSpeed = 2;

    public float WalkingSpeed { get => _walkingSpeed; set => _walkingSpeed = value; }
}
