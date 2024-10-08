import { Company } from "../../common/models/company";
import CompanyCard from "./CompanyCard";
import { Grid2 } from "@mui/material";
interface Props{
    data: Company[]
}
export default function CompanyList({data}: Props) {
  return(
    <Grid2 container spacing={4}>
        {data.map((company) => (
            <Grid2 xs={3} key={company.id}>
                <CompanyCard  company={company}/>  
            </Grid2>           
        ))}
    </Grid2>
  )
}