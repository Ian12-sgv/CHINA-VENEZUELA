import { useEffect } from 'react'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useQueryClient } from '@tanstack/react-query'
import { actualizacionesHubUrl, getAccessToken } from '../api'

const queryKeys = [
  ['compras-recibidas'],
  ['empresas'],
  ['marcas-bulto'],
  ['contenedores-compartidos'],
  ['aduanas'],
  ['puertos-llegada'],
  ['grupos'],
  ['usuarios'],
  ['receptores'],
] as const

export function useActualizacionesEnTiempoReal() {
  const queryClient = useQueryClient()

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(actualizacionesHubUrl, { withCredentials: false, accessTokenFactory: () => getAccessToken() ?? '' })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('DatosActualizados', () => {
      queryKeys.forEach(queryKey => {
        void queryClient.invalidateQueries({ queryKey })
      })
    })

    void connection.start().catch(() => undefined)

    return () => {
      connection.off('DatosActualizados')
      void connection.stop()
    }
  }, [queryClient])
}


