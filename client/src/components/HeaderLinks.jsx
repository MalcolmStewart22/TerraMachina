import { Link, useLocation } from "react-router-dom"

//TODO: UnComment WorldGen Link when it is in better state.
function HeaderLinks() {
    const location = useLocation()
    
    const linkClasses = (path) => {
        const base = "h-8 w-60 text-lg font-custom mt-4 pt-1 flex justify-center rounded-t-2xl"
        const active = "text-ink bg-elevated border-t border-r border-l border-accent"
        const inactive = "text-muted hover:text-ink"
        return `${base} ${location.pathname === path ? active : inactive}`
    }
  return(
    <div className="flex border-b-2 border-structureAccent bg-base">
      <div className="h-12 w-screen flex items-center ">
          <Link to="/" className="text-3xl px-4 font-title text-ink border-r-2 border-structureAccent h-full flex items-center">TerraMachina</Link>
          <Link to="/build" className={linkClasses("/build")}>Builder</Link>
          {/*<Link to="/gen"   className={linkClasses("/gen")}>Generator</Link>*/} 
          <Link to="/sim"   className={linkClasses("/sim")}>Simulator</Link>
      </div>
      <div className="flex justify-end items-end">
          <a href='https://studionex.dev'>
            <span className="text-xl text-structureAccent">StudioNex</span>
            <span className="text-accent">✦</span>
          </a>
      </div>
    </div> 
  ) 
}

export default HeaderLinks