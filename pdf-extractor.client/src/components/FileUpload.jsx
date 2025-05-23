import React, { useState, useEffect,useRef } from "react";
import {
    Container,
    Typography,
    Button,
    Box,
    List,
    ListItem,
    ListItemText,
    Input,
    Paper,
    Table,
    TableHead,
    TableRow,
    TableCell,
    TableBody,
} from "@mui/material";

function FileUpload() {
    const [selectedFile, setSelectedFile] = useState(null);
    const [metadataList, setMetadataList] = useState([]);
    const [searchQuery, setSearchQuery] = useState("");
    const fileInputRef = useRef(null);

    // Fetch metadata from the API
    useEffect(() => {
        console.log("calling metadata");
        fetch("https://localhost:32781/api/pdf/metadata")
            .then((res) => res.json())
            .then((data) => setMetadataList(data))
            .catch((error) => console.error("Error fetching metadata:", error));
    }, []);

    const handleFileChange = (e) => {
        setSelectedFile(e.target.files[0]);
    };

    const handleUpload = async () => {
        if (!selectedFile || selectedFile.type !== "application/pdf") {
            alert("Only PDF files are allowed.");
            return;
        }

        const formData = new FormData();
        formData.append("formFile", selectedFile);
        console.log("calling Upload");

        const response = await fetch("https://localhost:32781/api/pdf/upload", {
            method: "POST",
            body: formData,
        });

        if (response.ok) {
            const newMeta = await response.json();
            setMetadataList((prev) => [...prev, newMeta]);
            setSelectedFile(null);
            if (fileInputRef.current) {
                fileInputRef.current.value = "";
            }
        } else {
            alert("Upload failed. Please try again.");
        }
    };

    const handleSearch = async () => {
        if (!searchQuery.trim()) {
            alert("Please enter a search term.");
            return;
        }

        // Fetch search results from API
        const response = await fetch(
            `https://localhost:32781/api/pdf/search?query=${encodeURIComponent(searchQuery)}`
        );

        if (response.ok) {
            const result = await response.json();
            setMetadataList(result);
        } else {
            alert("No PDFs found matching your query.");
        }
    };

    return (
        <Container maxWidth="sm" sx={{ mt: 5 }}>
            <Paper elevation={3} sx={{ p: 3 }}>
                <Typography variant="h5" gutterBottom>
                    Upload PDF
                </Typography>

                <Box display="flex" alignItems="center" gap={2} my={2}>
                    <Input
                        type="file"
                        inputProps={{ accept: "application/pdf" }}
                        onChange={handleFileChange}
                        inputRef={fileInputRef}
                    />
                    <Button variant="contained" onClick={handleUpload}>
                        Upload
                    </Button>
                </Box>

                {/* Search Section */}
                <Typography variant="h6" gutterBottom>
                    Search PDFs
                </Typography>
                <Box display="flex" gap={2} my={2}>
                    <Input
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        placeholder="Search by title or author"
                    />
                    <Button variant="contained" onClick={handleSearch}>
                        Search
                    </Button>
                </Box>

                {/* Metadata List */}
                <Typography variant="h6" gutterBottom>
                    Metadata
                </Typography>
                <List dense>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell><strong>File Name</strong></TableCell>
                                <TableCell><strong>Title</strong></TableCell>
                                <TableCell><strong>Author</strong></TableCell>
                                <TableCell><strong>Page Count</strong></TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {metadataList.map((meta) => (
                                <TableRow key={meta.id}>
                                    <TableCell>{meta.fileName}</TableCell>
                                    <TableCell>{meta.title || "N/A"}</TableCell>
                                    <TableCell>{meta.author || "Unknown"}</TableCell>
                                    <TableCell>{meta.pageCount}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </List>
            </Paper>
        </Container>
    );
}

export default FileUpload;
