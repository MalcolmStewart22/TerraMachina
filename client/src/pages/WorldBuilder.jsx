import HeaderLinks from "../components/HeaderLinks"
import { generateSphere } from "../builder/Icosphere"



function WorldBuilder() {
  const cellMap = generateSphere()
  console.log(cellMap)
  
  return(
    <div className="bg-base text-ink min-h-screen flex justify-center">
      <HeaderLinks/>
    </div>
  ) 
}

export default WorldBuilder