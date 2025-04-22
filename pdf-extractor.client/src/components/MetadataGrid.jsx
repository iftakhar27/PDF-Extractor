import {
    Grid,
    Card,
    CardContent,
    Typography,
} from "@mui/material";

const MetadataGrid = ({ metadataList }) => {
    return (
        <Grid container spacing={3} padding={3}>
            {metadataList.map((item) => (
                <Grid item xs={12} sm={6} md={4} lg={3} key={item.id}>
                    <Card elevation={3}>
                        <CardContent>
                            <Typography variant="h6" gutterBottom>
                                {item.fileName}
                            </Typography>
                            <Typography variant="body2"><strong>Title:</strong> {item.title || "N/A"}</Typography>
                            <Typography variant="body2"><strong>Author:</strong> {item.author || "N/A"}</Typography>
                            <Typography variant="body2"><strong>Pages:</strong> {item.pageCount}</Typography>
                            <Typography variant="body2">
                                <strong>Created:</strong>{" "}
                                {item.createdDate ? new Date(item.createdDate).toLocaleDateString() : "N/A"}
                            </Typography>
                        </CardContent>
                    </Card>
                </Grid>
            ))}
        </Grid>
    );
};

export default MetadataGrid;
