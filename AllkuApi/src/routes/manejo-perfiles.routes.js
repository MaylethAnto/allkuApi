const express = require('express');
const router = express.Router();
const { db } = require('../config/firebase-config');

// Obtener todos los perfiles
router.get('/', async (req, res) => {
    try {
        const perfilesSnapshot = await db.collection('perfiles').get();
        const perfiles = [];
        perfilesSnapshot.forEach(doc => {
            perfiles.push({ id: doc.id, ...doc.data() });
        });
        res.json(perfiles);
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Obtener un perfil por ID
router.get('/:id', async (req, res) => {
    try {
        const perfilDoc = await db.collection('perfiles').doc(req.params.id).get();
        if (!perfilDoc.exists) {
            return res.status(404).json({ message: 'Perfil no encontrado' });
        }
        res.json({ id: perfilDoc.id, ...perfilDoc.data() });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Crear un nuevo perfil
router.post('/', async (req, res) => {
    try {
        const perfilData = req.body;
        const perfilRef = await db.collection('perfiles').add(perfilData);
        res.status(201).json({ id: perfilRef.id, ...perfilData });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Actualizar un perfil
router.put('/:id', async (req, res) => {
    try {
        const perfilRef = db.collection('perfiles').doc(req.params.id);
        const perfil = await perfilRef.get();
        if (!perfil.exists) {
            return res.status(404).json({ message: 'Perfil no encontrado' });
        }
        await perfilRef.update(req.body);
        res.json({ message: 'Perfil actualizado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

// Eliminar un perfil
router.delete('/:id', async (req, res) => {
    try {
        const perfilRef = db.collection('perfiles').doc(req.params.id);
        const perfil = await perfilRef.get();
        if (!perfil.exists) {
            return res.status(404).json({ message: 'Perfil no encontrado' });
        }
        await perfilRef.delete();
        res.json({ message: 'Perfil eliminado exitosamente' });
    } catch (error) {
        res.status(500).json({ error: error.message });
    }
});

module.exports = router;
