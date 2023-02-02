using UnityEngine;

namespace UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] protected GameObject Root;

        public virtual void Open()
        {
            Root.SetActive(true);
        }

        public virtual void Close()
        {
            Root.SetActive(false);
        }
    }
}