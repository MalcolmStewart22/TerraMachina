export async function startWorldGen(parameters){
    console.log(parameters)
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

export async function sendReadySignal(phase){
    let request = {phase: phase}
    const response = await fetch('https://localhost:7078/worldgen/ready', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
    })
    return response
}

export async function resetWorldGen(){
    const response = await fetch('https://localhost:7078/worldgen/reset', {
        method: 'POST',
        header: {
            'Content-Type': 'application/json'
        },
    })
}