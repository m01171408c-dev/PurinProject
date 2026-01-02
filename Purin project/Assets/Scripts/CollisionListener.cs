using System;
using UnityEngine;

public class CollisionListener : MonoBehaviour
{
    private string _thisTag = string.Empty;
    private string[] _targetTag = { };
    private Action<string,Collider> _onTriggerEnter = null;
    private bool _isGrounded = false;

    public bool IsGrounded => _isGrounded;
    public void SetUp(ref string[] targets,Action<string,Collider> onTriggerEnter)
    {
        _thisTag = gameObject.tag;
        _onTriggerEnter = onTriggerEnter;
        _targetTag = targets;
        _isGrounded = true; // Å‰‚Í’…’n‚Ì‘O’ñ
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_onTriggerEnter == null)
        {
            Debug.LogWarning(this.name + " CollisionListener: OnTriggerEnter called but _onTriggerEnter is null.");
            return;
        }
        if(other.tag == GameTag.Floor)
        {
            _isGrounded = true;
        }
        if(Array.Exists(_targetTag, tag => tag == other.tag))
        {
            _onTriggerEnter.Invoke(other.tag, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameTag.Floor)
        {
            _isGrounded = false;
        }
    }
}
