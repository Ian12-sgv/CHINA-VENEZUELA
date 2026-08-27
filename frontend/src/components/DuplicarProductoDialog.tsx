import { useEffect, useState, type FormEvent } from 'react'
import type { ProductoPedido } from '../types'

type DuplicarProductoDialogProps = {
  producto: ProductoPedido | null
  ocupada: boolean
  error: string
  onConfirm: (codigoBarraAsignado: string) => void
  onCancel: () => void
}

export function DuplicarProductoDialog({ producto, ocupada, error, onConfirm, onCancel }: DuplicarProductoDialogProps) {
  const [codigoBarraAsignado, setCodigoBarraAsignado] = useState('')

  useEffect(() => {
    if (producto) setCodigoBarraAsignado('')
  }, [producto])

  if (!producto) return null

  const confirmar = (event: FormEvent) => {
    event.preventDefault()
    const codigo = codigoBarraAsignado.trim()
    if (codigo) onConfirm(codigo)
  }

  return <div className="dialog-backdrop" role="presentation">
    <section className="duplicate-dialog" role="dialog" aria-modal="true" aria-labelledby="duplicate-title">
      <h3 id="duplicate-title">Duplicar subpedido</h3>
      <p>Se copiarán los datos de <strong>{producto.referenciaAsignada}</strong> al mismo grupo. Ingresa un código de barra nuevo.</p>
      <form onSubmit={confirmar}>
        <label>
          <span>Código de barra asignado</span>
          <input autoFocus value={codigoBarraAsignado} onChange={(event) => setCodigoBarraAsignado(event.target.value)} placeholder="Nuevo código de barra" maxLength={100} required disabled={ocupada} />
        </label>
        {error && <p className="error">{error}</p>}
        <div className="delete-actions">
          <button className="duplicate-confirm" type="submit" disabled={ocupada || !codigoBarraAsignado.trim()}>{ocupada ? 'Duplicando...' : 'Duplicar'}</button>
          <button className="cancel-confirm" type="button" disabled={ocupada} onClick={onCancel}>Cancelar</button>
        </div>
      </form>
      <small className="duplicate-note">Se copiaran automaticamente las imagenes de fabrica y del producto terminado.</small>
    </section>
  </div>
}
