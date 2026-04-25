import { Link, useLocation } from "react-router-dom"

//TODO: UnComment WorldGen Link when it is in better state.
function HeaderLinks() {
    const location = useLocation()
    
    const linkClasses = (path) => {
        const base = "h-8 w-60 text-lg font-custom mt-4 pt-1 flex justify-center rounded-t-2xl"
        const active = "text-ink bg-accent"
        const inactive = "text-muted hover:text-ink"
        return `${base} ${location.pathname === path ? active : inactive}`
    }
  return(
    <div className="h-12 w-screen flex items-center border-b-2 border-accent bg-elevated">
        <Link to="/" className="text-3xl p-4 font-title text-ink border-r-2 border-accent">TerraMachina</Link>
        <Link to="/build" className={linkClasses("/build")}>Builder</Link>
        {/*<Link to="/gen"   className={linkClasses("/gen")}>Generator</Link>*/} 
        <Link to="/sim"   className={linkClasses("/sim")}>Simulator</Link>
    </div> 
  ) 
}

export default HeaderLinks