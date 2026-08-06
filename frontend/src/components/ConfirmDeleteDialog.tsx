import { useEffect, useState } from 'react'

type ConfirmDeleteDialogProps = { open: boolean; itemName: string; onConfirm: () => void; onCancel: () => void }

export function ConfirmDeleteDialog({ open, itemName, onConfirm, onCancel }: ConfirmDeleteDialogProps) {
  const [confirming, setConfirming] = useState(false)
  useEffect(() => { if (!open) setConfirming(false) }, [open])
  if (!open) return null
  const confirm = () => { setConfirming(true); window.setTimeout(onConfirm, 360) }
  return <div className="dialog-backdrop" role="presentation"><section className={`delete-dialog ${confirming ? 'deleting' : ''}`} role="alertdialog" aria-modal="true" aria-labelledby="delete-title"><div className="delete-icon">!</div><h3 id="delete-title">Eliminar registro</h3><p>Vas a eliminar <strong>{itemName}</strong>. Esta accion no se puede deshacer.</p><div className="delete-actions"><button className="danger-confirm" disabled={confirming} onClick={confirm}>{confirming ? 'Eliminando...' : 'Esta seguro'}</button><button className="cancel-confirm" disabled={confirming} onClick={onCancel}>NO</button></div></section></div>
}