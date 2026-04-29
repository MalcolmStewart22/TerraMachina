import * as signalR from '@microsoft/signalr';
import { useEffect, useState } from 'react';

function useEngineHub() {
    const [latestUpdate, setLatestUpdate] = useState(null)

    useEffect(() =>{
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('terramachina-api.azurewebsites.net')
            .build()

        connection.on('WorldGenProgress', (update) => {
            setLatestUpdate(update)
        })
        
        connection.start().catch(err => console.error('SignalR connection failed:', err))

        return() => {
            connection.stop()
        }
    }, [])

    return latestUpdate
}
export default useEngineHub