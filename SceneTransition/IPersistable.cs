using System;

public interface IPersistable
{
    void PersistState();
    void ResetState();
    void RestoreState();
}
