import { Link } from "react-router-dom"


function Landing() {
  return(
    <div className="flex flex-col items-center justify-center min-h-screen gap-12 md:gap-20">
        <h1 className="text-4xl md:text-7xl p-4 font-title text-ink">TerraMachina</h1>
        <div className="flex flex-col xl:flex-row items-center justify-center gap-12 xl:gap-32">
            <Link to="/build" className="w-80 h-28 bg-elevated  border-accent border-2 text-ink text-3xl font-custom flex items-center justify-center rounded-2xl hover:bg-accent">Builder</Link>
            <Link to="/gen"   className="w-80 h-28 bg-elevated  border-accent border-2 text-muted text-3xl font-custom flex items-center justify-center rounded-2xl hover:bg-elevated">Generator</Link>
            <Link to="/sim"   className="w-80 h-28 bg-elevated  border-accent border-2 text-ink text-3xl font-custom flex items-center justify-center rounded-2xl hover:bg-accent">Simulator</Link>
        </div>
    </div> 
  ) 
}

export default Landing