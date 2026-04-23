import { Link, useLocation } from "react-router-dom"

//TODO: UnComment WorldGen Link when it is in better state.
function HeaderLinks() {
    const location = useLocation()
    
    const linkClasses = (path) => {
        const base = "h-12 w-60 text-2xl font-custom pb-2 flex items-end justify-center rounded-t-2xl"
        const active = "text-ink bg-accent"
        const inactive = "text-muted hover:text-ink"
        return `${base} ${location.pathname === path ? active : inactive}`
    }
  return(
    <div className="h-20 w-screen flex items-end border-b-2 border-accent bg-elevated">
        <Link to="/" className="text-5xl p-4 font-title text-ink flex items-center justify-center border-r-2 border-accent">TerraMachina</Link>
        <Link to="/build" className={linkClasses("/build")}>Builder</Link>
        {/*<Link to="/gen"   className={linkClasses("/gen")}>Generator</Link>*/} 
        <Link to="/sim"   className={linkClasses("/sim")}>Simulator</Link>
    </div> 
  ) 
}

export default HeaderLinks