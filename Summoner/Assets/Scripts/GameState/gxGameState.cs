using UnityEngine;
using System.Collections;

public abstract class gxGameState 
{

	bool							m_bStateLoading;

	public virtual bool				IsKindOf(int index){return false;}
	public virtual int Initalize() 		{ return 0;}
    public virtual int              Deinitialization() { return 0; }
    public virtual void				Release()	{ }
	public virtual void				Render()  	{ }
	public virtual void				Pause() 	{ }
	public virtual void				Resume()	{ }
	public virtual bool				Load() 		{ return false; }
    public virtual void             Exit()      { }
	public virtual bool				IsLoading() 	{ return m_bStateLoading; }
	public virtual void				SetLoading( bool loading ) { m_bStateLoading = loading; }

	public abstract void			Update(float dt);
    public virtual void             LateUpdate(float dt) { }
    public virtual void OnCompelete() { }

}
