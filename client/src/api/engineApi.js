export async function startWorldGen(seed, subdivisionLevel){
    const response = await fetch('https://localhost:7078/worldgen/start', {
        method: 'POST',
        headers:{
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ seed, subdivisionLevel })
        })
    return response
}