import { Avatar, Button, Card, CardActions, CardContent, CardHeader, CardMedia, Typography } from "@mui/material";
import { Company } from "../../common/models/company";
import { Link } from "react-router-dom";
import { LoadingButton } from "@mui/lab";
import { useState } from "react";

interface Props{
    company: Company
}

export default function CompanyCard({company}: Props) {
  const [loading, setLoading] = useState(false)

  return(
    <Card sx={{ maxWidth: 345 }}>
      <CardHeader 
        avatar={
          <Avatar sx={{ bgcolor: 'primary.main'}}>
            {company.name.charAt(0).toUpperCase()}
          </Avatar>
        }
        title={company.name} 
        subheader={company.address}
        titleTypographyProps={{
          sx: { fontSize: 20, fontWeight: 'bold',color: 'primary.main' }
        }} />
      <CardMedia
        sx={{ height: 140 ,backgroundColor: 'primary.light' }}
        image="http://picsum.photos/id/1018/200/300"
        title={company.name}
      />
      <CardContent>
        <Typography gutterBottom color='secondary' variant="h5" >
          {company.address}
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary' }}>
          Lizards are a widespread group of squamate reptiles, with over 6,000
          species, ranging across all continents except Antarctica
        </Typography>
      </CardContent>
      <CardActions>
        <LoadingButton loading={loading} 
                       onClick={() => setLoading(!loading)} 
                       size="small">Share</LoadingButton>
        <Button component={Link}  to={`/catalog/${company.id}`} size="small">Learn More</Button>
      </CardActions>
    </Card>
  )
}