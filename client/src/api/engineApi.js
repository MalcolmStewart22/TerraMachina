export async function startWorldGen(parameters){
    
    const response = await fetch('https://localhost:7078/worldgen/start', {
        method: 'POST',
        headers:{
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(parameters)
        })
    console.log(response)    
    return response
}

export async function sendReadySignal(){
    const response = await fetch('https://localhost:7078/worldgen/ready', {
        method: 'POST',
        header: {
            'Content-Type': 'application/json'
        },
    })
}